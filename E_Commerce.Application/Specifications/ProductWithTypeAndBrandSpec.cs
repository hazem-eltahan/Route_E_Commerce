using E_Commerce.Application.Common;
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
        public ProductWithTypeAndBrandSpec(ProductQueryParams productQueryParams)
            :base(p => (!productQueryParams.BrandId.HasValue || p.BrandId == productQueryParams.BrandId.Value)
            && (!productQueryParams.TypeId.HasValue || p.TypeId == productQueryParams.TypeId.Value)
            && (string.IsNullOrWhiteSpace(productQueryParams.Searchvalue) || p.Name.ToLower().Contains(productQueryParams.Searchvalue.ToLower())))
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
