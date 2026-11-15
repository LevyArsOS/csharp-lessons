# Alocações de String em Textos de Combate e HUD

## Problema
Jogos de ação, roguelikes e estratégia frequentemente exibem centenas ou milhares de números de dano flutuantes e contadores de HUD por frame (`"#42 -185 (815/1000)"`). Montar essas strings a cada frame usando concatenação (`+`) ou interpolação (`$"..."`) aloca múltiplos objetos `string` temporários na heap gerenciada (`549.880 B` para `2.000` eventos), disparando coletas frequentes na Gen0.

## Solução
1. **`Span<char>` + `TryFormat`:** Formate os números diretamente em um buffer reutilizável (`char[]` ou `stackalloc char[64]`) usando `int.TryFormat(..., CultureInfo.InvariantCulture)` e passe `ReadOnlySpan<char>` para o renderizador de texto (`0 B` alocados, `3.59x` mais rápido).
2. **`StringBuilder` Reutilizável:** Quando um buffer mutável de caracteres for necessário, chame `Clear()` e reutilize uma única instância de `StringBuilder` (`0 B` alocados, `1.49x` mais rápido).
3. **Tabelas de Busca Pré-Alocadas:** Quando uma API legada de UI exigir uma referência `string` e os valores estiverem em uma faixa conhecida (`0..999`), pré-aloque as strings na inicialização (`0 B` alocados, `123.65x` mais rápido).

## Resumo do Benchmark
No .NET 9 (`2.000` eventos de combate por frame), `Span<char>` com `TryFormat` reduz o tempo de `204.518 us` (`549.880 B` alocados) para `56.945 us` (`0 B` alocados, `3.59x` mais rápido).
