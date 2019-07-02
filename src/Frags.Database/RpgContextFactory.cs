using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frags.Database
{
    public class RpgContextFactory : IDesignTimeDbContextFactory<RpgContext>
    {
        public RpgContext CreateDbContext(string[] args)
        {
            var options = new GeneralOptions
            {
                UseInMemoryDatabase = false,
                DatabaseName = "DesignTimeDebugDB"
            };

            return new RpgContext(options);
        }
    }
}
