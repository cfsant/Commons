using Commons.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Commons.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable(nameof(Profile));

            entityTypeBuilder.HasIndex(x => x.Name).IsUnique();
            entityTypeBuilder.Property(x => x.State).HasDefaultValue(true);
            entityTypeBuilder.Property(x => x.Insertion).HasDefaultValue(DateTime.Now);
            entityTypeBuilder.Property(x => x.Change).HasDefaultValue(DateTime.MinValue);
            entityTypeBuilder.Property(x => x.OwnerId).HasDefaultValue(Guid.Empty);
            entityTypeBuilder.Property(x => x.PublisherId).HasDefaultValue(Guid.Empty);
        }
    }
}
