using Products.API.Models;

namespace Products.API.Specifications
{
    public class ProductsWithDetailsSpecification : BaseSpecification<Product>
    {
        public ProductsWithDetailsSpecification()
        {
            AddInclude(p => p.Brand);
            AddInclude(p => p.Category);
            AddInclude(p => p.Mount);
        }

        public ProductsWithDetailsSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.Brand);
            AddInclude(p => p.Category);
            AddInclude(p => p.Mount);
        }
    }
}
