using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.infraestrutura.service.Extensions;
using umfgcloud.infraestrutura.service.Maps;

namespace umfgcloud.infraestrutura.service.Context;

public sealed class MySqlDataBaseContext : IdentityDbContext
{
    private const string C_IN_MEMORY_PROVIDER
        = "Microsoft.EntityFrameworkCore.InMemory";

    public MySqlDataBaseContext(DbContextOptions<MySqlDataBaseContext> options)
        : base(options)
    {
        ApplyMigrations();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureToMySQL();

        builder.ApplyConfiguration(new ProdutoMap());
    }

    private void ApplyMigrations()
    {
        if (IsInMemory())
            return;

        if (Database.GetPendingMigrations().Any())
            Database.Migrate();
    }

    private bool IsInMemory()
        => C_IN_MEMORY_PROVIDER.Equals(Database.ProviderName);
}