using EShopSolution.Data.Domain;
using EShopSolution.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Data.Entities
{
    public class Contact : DomainEntity<int>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public Status Status { get; set; }
    }
}
