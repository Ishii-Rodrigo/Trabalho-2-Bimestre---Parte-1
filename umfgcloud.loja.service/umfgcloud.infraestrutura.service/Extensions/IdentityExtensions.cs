using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umfgcloud.infraestrutura.service.Extensions
{
    internal static class IdentityExtensions
    {
        internal static void ConfigureToMySQL(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUser>(x =>
            {
                x.Property(y => y.Id).HasMaxLength(110);
                x.Property(y => y.Email).HasMaxLength(127);
                x.Property(y => y.NormalizedEmail).HasMaxLength(127);
                x.Property(y => y.UserName).HasMaxLength(127);
                x.Property(y => y.NormalizedUserName).HasMaxLength(127);
            });

            modelBuilder.Entity<IdentityRole>(x =>
            {
                x.Property(y => y.Id).HasMaxLength(110);
                x.Property(y => y.Name).HasMaxLength(127);
                x.Property(y => y.NormalizedName).HasMaxLength(127);
            });

            modelBuilder.Entity<IdentityUserRole<string>>(x =>
            {
                x.Property(y => y.RoleId).HasMaxLength(110);
                x.Property(y => y.UserId).HasMaxLength(110);
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(x =>
            {
                x.Property(y => y.LoginProvider).HasMaxLength(127);
                x.Property(y => y.ProviderKey).HasMaxLength(127);
            });

            modelBuilder.Entity<IdentityUserToken<string>>(x =>
            {
                x.Property(y => y.UserId).HasMaxLength(110);
                x.Property(y => y.LoginProvider).HasMaxLength(127);
                x.Property(y => y.Name).HasMaxLength(127);
            });
        }
    }
}
