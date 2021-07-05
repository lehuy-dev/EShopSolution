using EShopSolution.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Data.Entities
{
    public class Product : DomainEntity<int>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int Stock { get; set; }
        public int ViewCount { get; set; }
        public DateTime DateCreated { get; set; }
        public bool? IsFeature { get; set; }
        public List<ProductInCategory> ProductInCategories { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public List<Cart> Carts { get; set; }
        public List<ProductTranslation> ProductTranslations { get; set; }
        public List<ProductImage> ProductImages { get; set; }
    }
}
