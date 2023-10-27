namespace OktaBoiler.Infrastructure
{
    public class CacheItem<T>
    {
        public DateTime Created { get; set; } = DateTime.Now;
        public T Value { get; set; }
        public CacheItem(T value)
        {
            Value = value;
        }
    }
}
