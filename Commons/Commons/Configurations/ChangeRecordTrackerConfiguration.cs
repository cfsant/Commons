using Commons.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Commons.Configurations
{
    public class ChangeRecordTrackerConfiguration : IEntityTypeConfiguration<ChangeRecordTracker>
    {
        public void Configure(EntityTypeBuilder<ChangeRecordTracker> entityTypeBuilder)
        {
            entityTypeBuilder.ToTable(nameof(ChangeRecordTracker));

            entityTypeBuilder.Property(x => x.Insertion).HasDefaultValue(DateTime.Now);
            entityTypeBuilder.Property(x => x.Insertion).HasDefaultValue(DateTime.Now);
        }
    }
}
