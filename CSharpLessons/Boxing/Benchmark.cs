using BenchmarkDotNet.Attributes;
using CSharpLessons.Boxing.Data;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpLessons.Boxing
{
    [MemoryDiagnoser]
    public class Benchmark
    {
        [Params(100, 1_000, 10_000)] public int Size;

        private List<DataWithSpecializedOverride> specializedOverrides = null!;
        private List<DataWithoutSpecializedOverride> withoutSpecializedOverrides = null!;
        private List<Point> points = null!;
        private List<Coordinate> coordinates = null!;
        private List<Product> products = null!;
        private List<Temperature> temperatures = null!;
        private ArrayList arrayList = null!;
        private int[] tupleData = null!;

        [GlobalSetup]
        public void Setup()
        {
            specializedOverrides = new List<DataWithSpecializedOverride>(Size);
            withoutSpecializedOverrides = new List<DataWithoutSpecializedOverride>(Size);
            points = new List<Point>(Size);
            coordinates = new List<Coordinate>(Size);
            products = new List<Product>(Size);
            temperatures = new List<Temperature>(Size);
            arrayList = new ArrayList(Size);
            tupleData = new int[Size];

            var random = new Random(42);
            for (int i = 0; i < Size; i++)
            {
                specializedOverrides.Add(new DataWithSpecializedOverride { Id = i, Velocity = random.NextSingle() * 100 });
                withoutSpecializedOverrides.Add(new DataWithoutSpecializedOverride { Id = i, Velocity = random.NextSingle() * 100 });
                points.Add(new Point { X = random.Next(100), Y = random.Next(100) });
                coordinates.Add(new Coordinate { X = random.Next(100), Y = random.Next(100) });
                products.Add(new Product { Id = i, Price = random.Next(1, 100) });
                temperatures.Add(new Temperature { Celsius = random.NextDouble() * 100 });
                arrayList.Add(withoutSpecializedOverrides[i]); // Boxing occurs here
                tupleData[i] = random.Next(100);
            }
        }

        // GetHashCode benchmarks
        [Benchmark(Baseline = true)]
        public void HashCalculationSpecialized()
        {
            int hash = 0;
            foreach (var data in specializedOverrides)
            {
                hash += data.GetHashCode();
            }
        }

        [Benchmark]
        public void HashCalculationDefault()
        {
            int hash = 0;
            foreach (var data in withoutSpecializedOverrides)
            {
                hash += data.GetHashCode();
            }
        }

        // Equals benchmarks - IEquatable<T> vs object.Equals
        [Benchmark]
        public void EqualsWithIEquatable()
        {
            for (int i = 0; i < Size; i++)
            {
                var a = specializedOverrides[i];
                var b = specializedOverrides[i];
                a.Equals(b); // No boxing - uses IEquatable<T>
            }
        }

        [Benchmark]
        public void EqualsWithObject()
        {
            for (int i = 0; i < Size; i++)
            {
                var a = specializedOverrides[i];
                var b = specializedOverrides[i];
                a.Equals((object)b); // Boxing - uses object.Equals
            }
        }

        [Benchmark]
        public void EqualsDefault()
        {
            for (int i = 0; i < Size; i++)
            {
                var a = withoutSpecializedOverrides[i];
                var b = withoutSpecializedOverrides[i];
                a.Equals(b); // Boxing - uses default object.Equals
            }
        }

        // ToString benchmarks
        [Benchmark]
        public void ToStringSpecialized()
        {
            foreach (var data in specializedOverrides)
            {
                _ = data.ToString(); // No boxing
            }
        }

        [Benchmark]
        public void ToStringDefault()
        {
            foreach (var data in withoutSpecializedOverrides)
            {
                _ = data.ToString(); // Boxing
            }
        }

        // HashSet/Dictionary benchmarks
        [Benchmark]
        public void HashSetWithIEquatable()
        {
            var hashSet = new HashSet<Point>();
            foreach (var point in points)
            {
                hashSet.Add(point); // No boxing - uses IEquatable<T>
            }
        }

        [Benchmark]
        public void HashSetWithObject()
        {
            var hashSet = new HashSet<object>();
            foreach (var point in points)
            {
                hashSet.Add(point); // Boxing occurs here!
            }
        }

        // IComparable<T> vs IComparable benchmarks
        [Benchmark]
        public void CompareToGeneric()
        {
            for (int i = 0; i < Size - 1; i++)
            {
                temperatures[i].CompareTo(temperatures[i + 1]); // No boxing - uses IComparable<T>
            }
        }

        [Benchmark]
        public void CompareToNonGeneric()
        {
            for (int i = 0; i < Size - 1; i++)
            {
                IComparable comp = temperatures[i]; // Boxing here!
                comp.CompareTo(temperatures[i + 1]); // Additional boxing
            }
        }

        // LINQ benchmarks with structs
        [Benchmark]
        public void DistinctWithoutComparer()
        {
            _ = coordinates.Distinct().ToList(); // Boxing in each comparison
        }

        [Benchmark]
        public void DistinctWithComparer()
        {
            _ = coordinates.Distinct(new CoordinateEqualityComparer()).ToList(); // No boxing
        }

        [Benchmark]
        public void ContainsWithoutIEquatable()
        {
            var coordinate = coordinates[0];
            for (int i = 0; i < Size; i++)
            {
                _ = coordinates.Contains(coordinate); // Boxing in comparison
            }
        }

        [Benchmark]
        public void ContainsWithIEquatable()
        {
            var point = points[0];
            for (int i = 0; i < Size; i++)
            {
                _ = points.Contains(point); // No boxing if Point implements IEquatable<Point>
            }
        }

        // ArrayList benchmarks
        [Benchmark]
        public void ArrayListContains()
        {
            var data = withoutSpecializedOverrides[0];
            for (int i = 0; i < Size; i++)
            {
                _ = arrayList.Contains(data); // Boxing when passing and in each comparison
            }
        }

        // Product sorting benchmarks
        [Benchmark]
        public void SortWithIComparableT()
        {
            var productsCopy = new List<Product>(products);
            productsCopy.Sort(); // No boxing - uses IComparable<Product>
        }

        [Benchmark]
        public void OrderByWithIComparableT()
        {
            _ = products.OrderBy(p => p).ToList(); // No boxing
        }

        // String Interpolation benchmarks
        [Benchmark]
        public void StringInterpolationWithBoxing()
        {
            for (int i = 0; i < Size; i++)
            {
                int valor = specializedOverrides[i].Id;
                float velocidade = specializedOverrides[i].Velocity;
                // Boxing occurs - compiler converts to object internally
                _ = $"Valor: {valor}, Velocidade: {velocidade}";
            }
        }

        [Benchmark]
        public void StringInterpolationWithoutBoxing()
        {
            for (int i = 0; i < Size; i++)
            {
                int valor = specializedOverrides[i].Id;
                float velocidade = specializedOverrides[i].Velocity;
                // No boxing - explicit conversion to string
                _ = $"Valor: {valor.ToString()}, Velocidade: {velocidade.ToString()}";
            }
        }

        [Benchmark]
        public void StringInterpolationWithFormatting()
        {
            for (int i = 0; i < Size; i++)
            {
                int valor = specializedOverrides[i].Id;
                float velocidade = specializedOverrides[i].Velocity;
                // Boxing in both values with alignment
                _ = $"|{valor,10}|{velocidade,10}|";
            }
        }

        // Reflection benchmarks
        [Benchmark]
        public void GetTypeOnValueType()
        {
            for (int i = 0; i < Size; i++)
            {
                int numero = specializedOverrides[i].Id;
                // Boxing occurs - GetType() is called on a value type
                _ = numero.GetType();
            }
        }

        [Benchmark]
        public void TypeofForKnownType()
        {
            for (int i = 0; i < Size; i++)
            {
                // No boxing - use typeof for known types
                _ = typeof(int);
            }
        }

        [Benchmark]
        public void GetTypeInGenericMethod()
        {
            for (int i = 0; i < Size; i++)
            {
                ProcessValueWithGetType(specializedOverrides[i]);
            }
        }

        [Benchmark]
        public void TypeofInGenericMethod()
        {
            for (int i = 0; i < Size; i++)
            {
                ProcessValueWithTypeof<DataWithSpecializedOverride>(specializedOverrides[i]);
            }
        }

        // Helper methods for reflection benchmarks
        private void ProcessValueWithGetType<T>(T value) where T : struct
        {
            // Boxing here!
            _ = value.GetType();
        }

        private void ProcessValueWithTypeof<T>(T value) where T : struct
        {
            // No boxing
            _ = typeof(T);
        }

        // ValueTuple vs Tuple benchmarks
        [Benchmark]
        public void TupleCreation()
        {
            for (int i = 0; i < Size; i++)
            {
                var tuple = Tuple.Create(tupleData[i], $"value{i}"); // Boxing when creating Tuple
            }
        }

        [Benchmark]
        public void ValueTupleCreation()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = (tupleData[i], $"value{i}"); // No boxing
            }
        }

        [Benchmark]
        public void TupleInList()
        {
            var list = new List<Tuple<int, string>>(Size);
            for (int i = 0; i < Size; i++)
            {
                list.Add(Tuple.Create(tupleData[i], $"value{i}")); // Boxing when creating Tuple
            }
        }

        [Benchmark]
        public void ValueTupleInList()
        {
            var list = new List<(int, string)>(Size);
            for (int i = 0; i < Size; i++)
            {
                list.Add((tupleData[i], $"value{i}")); // No boxing
            }
        }

        [Benchmark]
        public void TupleAsDictionaryKey()
        {
            var dict = new Dictionary<Tuple<int, int>, string>(Size);
            for (int i = 0; i < Size; i++)
            {
                var key = Tuple.Create(tupleData[i], tupleData[i] + 1);
                dict[key] = $"value{i}"; // Boxing when creating Tuple
            }
        }

        [Benchmark]
        public void ValueTupleAsDictionaryKey()
        {
            var dict = new Dictionary<(int, int), string>(Size);
            for (int i = 0; i < Size; i++)
            {
                var key = (tupleData[i], tupleData[i] + 1);
                dict[key] = $"value{i}"; // No boxing
            }
        }

        [Benchmark]
        public void TupleReturn()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = GetTuple(tupleData[i], $"value{i}"); // Boxing when creating Tuple
            }
        }

        [Benchmark]
        public void ValueTupleReturn()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = GetValueTuple(tupleData[i], $"value{i}"); // No boxing
            }
        }

        private Tuple<int, string> GetTuple(int id, string name) => Tuple.Create(id, name);
        private (int, string) GetValueTuple(int id, string name) => (id, name);

        // ValueTask vs Task benchmarks
        [Benchmark]
        public async Task TaskWithSynchronousResult()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = await GetTaskSynchronous(tupleData[i]); // Allocates Task even with ready result
            }
        }

        [Benchmark]
        public async Task ValueTaskWithSynchronousResult()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = await GetValueTaskSynchronous(tupleData[i]); // No allocation - struct on stack
            }
        }

        [Benchmark]
        public async Task TaskWithAsyncResult()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = await GetTaskAsync(tupleData[i]); // Allocates Task
            }
        }

        [Benchmark]
        public async Task ValueTaskWithAsyncResult()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = await GetValueTaskAsync(tupleData[i]); // May allocate Task internally
            }
        }

        [Benchmark]
        public void TaskFromResult()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = Task.FromResult(tupleData[i]); // Allocates Task on heap
            }
        }

        [Benchmark]
        public void ValueTaskFromResult()
        {
            for (int i = 0; i < Size; i++)
            {
                _ = new ValueTask<int>(tupleData[i]); // No allocation - struct on stack
            }
        }

        [Benchmark]
        public async Task TaskInList()
        {
            var tasks = new List<Task<int>>(Size);
            for (int i = 0; i < Size; i++)
            {
                tasks.Add(GetTaskSynchronous(tupleData[i])); // Allocates Task
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task ValueTaskInList()
        {
            var valueTasks = new List<ValueTask<int>>(Size);
            for (int i = 0; i < Size; i++)
            {
                valueTasks.Add(GetValueTaskSynchronous(tupleData[i])); // No allocation
            }
            await Task.WhenAll(valueTasks.Select(vt => vt.AsTask()));
        }

        private Task<int> GetTaskSynchronous(int value) => Task.FromResult(value);
        private ValueTask<int> GetValueTaskSynchronous(int value) => new ValueTask<int>(value);
        private async Task<int> GetTaskAsync(int value)
        {
            await Task.Yield();
            return value;
        }
        private async ValueTask<int> GetValueTaskAsync(int value)
        {
            await Task.Yield();
            return value;
        }
    }
}
