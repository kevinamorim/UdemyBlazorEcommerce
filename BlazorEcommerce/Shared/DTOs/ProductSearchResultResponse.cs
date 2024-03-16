namespace BlazorEcommerce.Shared.DTOs
{
    public class ProductSearchResultResponse
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
