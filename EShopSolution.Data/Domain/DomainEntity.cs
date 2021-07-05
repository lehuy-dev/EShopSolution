using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Data.Domain
{
    public class DomainEntity<T>
    {
        public T Id { get; set; }
    }
}
