namespace FinalProjectMvc.Models
{
    public class ReportViewModel
    {
        public List<Product> OutOfStockProducts { get; set; }
        public List<Product> ProductsWithNoReviews { get; set; }
        public List<MostReviewedResult> MostReviewedProducts { get; set; }
        public List<Product> TopRatedProducts { get; set; }
    }

    public class MostReviewedResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ReviewCount { get; set; }
    }
}
