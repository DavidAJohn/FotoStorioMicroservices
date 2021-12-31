using Products.API.Helpers;
using Products.API.Models;

namespace Products.API.Specifications
{
    public class ProductsWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductsWithFiltersForCountSpecification(ProductSpecificationParams productParams)
            : base(p =>
                (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search)) &&
                (!productParams.BrandId.HasValue || p.BrandId == productParams.BrandId) &&
                (!productParams.CategoryId.HasValue || p.CategoryId == productParams.CategoryId) &&
                (!productParams.MountId.HasValue || p.MountId == productParams.MountId)
            )
        {
        }
    }
}
