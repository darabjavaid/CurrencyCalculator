namespace CurrencyCalculator.Models
{
    public class PaginatedResponse<T>
    {
        public string Base { get; set; }
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
    }

}
