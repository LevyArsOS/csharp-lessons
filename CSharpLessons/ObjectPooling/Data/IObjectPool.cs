namespace CSharpLessons.ObjectPooling.Data
{
    public interface IObjectPool<T> where T : class
    {
        int Capacity { get; }
        int ActiveCount { get; }
        int AvailableCount { get; }
        T Rent();
        void Return(T item);
    }
}
