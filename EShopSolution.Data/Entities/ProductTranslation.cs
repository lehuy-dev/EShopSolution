using EShopSolution.Data.Domain;
using EShopSolution.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Data.Entities
{
    public class ProductTranslation : DomainEntity<int>,IHasSeoMetaData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public int LanguageId { get; set; }
        public Language Language { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string SeoPageTitle { get;set; }
        public string SeoAlias { get;set; }
        public string SeoKeywords { get;set; }
        public string SeoDescription { get;set; }
    }
}
