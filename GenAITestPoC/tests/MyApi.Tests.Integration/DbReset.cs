using Microsoft.Extensions.DependencyInjection;
using MyApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApi.Tests.Integration
{
    public static class DbReset
    {
        public static void Reset(CustomWebAppFactory factory)
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
