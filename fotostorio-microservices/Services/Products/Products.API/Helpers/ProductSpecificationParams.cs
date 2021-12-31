namespace Products.API.Helpers
{
    public class ProductSpecificationParams
    {
        private const int MaxPageSize = 20;
        public int PageIndex { get; set; } = 1;

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public int? MountId { get; set; }
        public string Sort { get; set; }
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value.ToLower();
        }

        public bool IsAvailable { get; set; } = true;
    }
}
