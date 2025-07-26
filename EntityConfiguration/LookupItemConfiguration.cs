using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.EntityConfiguration
{
    public class LookupItemConfiguration : IEntityTypeConfiguration<LookupItem>
    {
        public void Configure(EntityTypeBuilder<LookupItem> builder)
        {
            builder.ToTable("LookupItems");

            // Id - IsDeleted - CreationDate
            builder.Property(x => x.Id)
                   .IsRequired()
                   .HasMaxLength(5)
                   .ValueGeneratedNever();

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(x => x.CreationDate)
                   .HasDefaultValueSql("GETDATE()");


            // Name - Code - IconPath
            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.Code)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(x => x.IconPath)
                   .HasMaxLength(300);


            // Seed Date
            builder.HasData(
                new LookupItem
                {
                    Id = "1",
                    Name = "InactiveItem",
                    Code = "I",
                    IconPath = "/StaticFiles/inactive-icon.png",
                    LookupTypeId = "1"
                },
                new LookupItem
                {
                    Id = "2",
                    Name = "ActiveItem",
                    Code = "A",
                    IconPath = "/StaticFiles/active-icon.png",
                    LookupTypeId = "1"
                },
                new LookupItem
                {
                    Id = "3",
                    Name = "NewItem",
                    Code = "N",
                    IconPath = "/StaticFiles/new-icon.png",
                    LookupTypeId = "1"
                },
                new LookupItem
                {
                    Id = "4",
                    Name = "FocusedItem",
                    Code = "F",
                    IconPath = "/StaticFiles/Focused-icon.png",
                    LookupTypeId = "1"
                }
            );
        }
    }
}
