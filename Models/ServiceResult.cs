namespace MoneySaver.SPA.Models
{
    public class ServiceResult<T> where T : notnull
    {
        public ServiceResult()
        {
            this.Errors = new List<string>();
        }

        public T Result { get; set; }

        public List<string> Errors { get; set; }
    }
}
