namespace MoneySaver.SPA.Models
{
    public class IdAndValue<T>
    {
        public int Id { get; set; }

        public T Value { get; set; }
    }
}
