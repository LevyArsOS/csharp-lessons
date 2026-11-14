# Object Pooling em C#

## Por que `new Projectile()` gera engasgos de frame
Em jogos a 60 FPS, criar e descartar centenas de projéteis ou partículas por segundo com `new Projectile()` enche a geração Gen0 rapidamente. Mesmo com a alocação rápida do .NET 9, gerar `~804 KB` de lixo a cada 20.000 disparos força coletas Gen0 e Gen1 que disputam o orçamento de 16,6 ms por frame.

## `IObjectPool<T>` Pré-Alocado
`ObjectPool<T>` pré-aloca `capacity` instâncias em uma `Stack<T>` no carregamento da cena. `Rent()` retira uma instância em $O(1)$ sem alocar no heap, e `Return(item)` reseta todos os campos de forma determinística antes de devolvê-la:

```csharp
public T Rent()
{
    if (_available.Count == 0)
        throw new InvalidOperationException("Object pool exhausted.");
    ActiveCount++;
    return _available.Pop();
}

public void Return(T item)
{
    ArgumentNullException.ThrowIfNull(item);
    if (_available.Count >= Capacity)
        throw new InvalidOperationException("Cannot return item to a full pool.");
    _onReset(item);
    _available.Push(item);
    ActiveCount--;
}
```

## Pontos Principais
- Sempre limpe todos os campos mutáveis em `onReset` para evitar estado residual.
- Exponha `ActiveCount` e `AvailableCount` para dimensionar a capacidade do pool com telemetria real.
- Use remoção swap-and-pop na lista `List<Projectile>` de ativos para manter o despawn em $O(1)$.
