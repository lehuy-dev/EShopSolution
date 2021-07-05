using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShopSolution.Data.EF
{
    public class EShopDbContextFactory : IDesignTimeDbContextFactory<EShopDbContext>
    {
        public EShopDbContext CreateDbContext (string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("EShopSolution");
            var optionBuilder = new DbContextOptionsBuilder<EShopDbContext>();
            optionBuilder.UseSqlServer(connectionString);
            return new EShopDbContext(optionBuilder.Options);
        }
    }
}
