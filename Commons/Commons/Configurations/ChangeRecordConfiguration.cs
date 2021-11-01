using Commons.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Commons.Configurations
{
    public class ChangeRecordConfiguration : IEntityTypeConfiguration<ChangeRecord>
    {
        public void Configure(EntityTypeBuilder<ChangeRecord> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable(nameof(ChangeRecord));

            entityTypeBuilder.Property(x => x.State).HasDefaultValue(true);
            entityTypeBuilder.Property(x => x.Insertion).HasDefaultValue(DateTime.Now);
            entityTypeBuilder.Property(x => x.Change).HasDefaultValue(DateTime.MinValue);
        }
    }
}
