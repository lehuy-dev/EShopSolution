using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Data.Interfaces
{
    public interface IHasSeoMetaData
    {
        string SeoPageTitle { get; set; }
        string SeoAlias { get; set; }
        string SeoKeywords { get; set; }
        string SeoDescription { get; set; }
    }
}
