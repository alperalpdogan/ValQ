using ValQ.Core.Domain.Common;
using ValQ.Core.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValQ.Core.Domain.Game;
using ValQ.Core.Domain.Logging;
using ValQ.Core.Domain.Localization;

namespace ValQ.Data.Context
{
    public class ValQDbContext : IdentityDbContext<User, Role, string>
    {
        public ValQDbContext(DbContextOptions<ValQDbContext> options) : base(options)
        {

        }

        public DbSet<Character> Character { get; set; }

        public DbSet<Weapon> Weapon { get; set; }

        public DbSet<Skill> Skill { get; set; }

        public DbSet<Origin> Origin { get; set; }

        public DbSet<WeaponDamage> WeaponDamage { get; set; }

        public DbSet<QuestionTemplate> QuestionTemplate { get; set; }

        public DbSet<Log> Log { get; set; }

        public DbSet<Language> Language { get; set; }

        public DbSet<LocaleStringResource> LocaleStringResource { get; set; }

        public DbSet<LocalizedProperty> LocalizedProperty { get; set; }

        public DbSet<Match> Match { get; set; }

        public DbSet<Rank> Rank { get; set; }

        public DbSet<UserRankHistory> UserRankHistory { get; set; }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeletedEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.Entity.Deleted = true;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDeletedEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.Entity.Deleted = true;
                        break;
                }
            }
            return base.SaveChanges();
        }
    }
}
