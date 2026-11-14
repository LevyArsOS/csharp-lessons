# Alocações no Hot Path (Loops de Atualização Zero-Allocation)

## Por que LINQ por Frame Quebra os 60 FPS
Chamar `.Where(...).Select(...).ToList()` dentro do `Update()` na Unity ou `_Process()` na Godot aloca memória em todo frame:
- Iteradores do LINQ (`WhereArrayIterator`, `SelectEnumerableIterator`)
- Classes de closure quando variáveis locais são capturadas
- Um novo `List<int>` e realocações internas a cada tick

Em `1.000` entidades durante `60` frames, isso gera `149.280 B` de lixo temporário por segundo.

## O Padrão `NonAlloc` com Reúso de Buffer
Seguindo a mesma ideia de `Physics.RaycastNonAlloc` na Unity e reúso com `List<T>.Clear()`, passe um buffer `List<int>` pré-alocado e use laços `for` indexados:

```csharp
public static void QueryActiveNonAlloc(Entity[] entities, float minEnergy, List<int> results)
{
    results.Clear();
    for (int i = 0; i < entities.Length; i++)
    {
        Entity e = entities[i];
        if (e.Active && e.Energy >= minEnergy)
            results.Add(e.Id);
    }
}
```

## Pontos Principais
- Mantenha buffers `List<T>` como campos privados nos sistemas e chame `Clear()` (que zera `_size` sem liberar o array interno).
- Evite LINQ em `Update()` / `FixedUpdate()` / `_Process()`; use LINQ apenas em caminhos frios (carregamento, editor, ferramentas).
- Faça cache de referências de componentes e arrays em `Awake()` / `_Ready()` em vez de buscar a cada frame.
