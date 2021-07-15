using EShopSolution.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Data.Entities
{
    public class Cart : DomainEntity<int>
    { 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public Guid UserID { get; set; }
        public AppUser AppUser { get; set; }
    }
}
