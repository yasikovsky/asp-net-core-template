namespace ProjectNameApi.Models
{
    public class FilteredQueryModel<T>
    {
        public T SearchFilter { get; set; }
        public T ResultFilter { get; set; }
    }
}