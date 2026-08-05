using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal class ProductWithTypeAndBrandSpec : BaseSpecification<Product, int>
    {
        public ProductWithTypeAndBrandSpec(int? BrandId, int? TypeId)
            :base(p => (!BrandId.HasValue || p.BrandId == BrandId.Value) && (!TypeId.HasValue || p.TypeId == TypeId.Value))
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
        public ProductWithTypeAndBrandSpec(int id) : base(x=>x.Id == id)
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
    }
}
