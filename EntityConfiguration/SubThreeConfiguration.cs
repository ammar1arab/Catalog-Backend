using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.EntityConfiguration
{
    public class SubThreeConfiguration : IEntityTypeConfiguration<SubThree>
    {
        public void Configure(EntityTypeBuilder<SubThree> builder)
        {
            builder.ToTable("SubThrees");

            // Id - IsDeleted - CreationDate
            builder.Property(x => x.Id)
                   .IsRequired()
                   .HasMaxLength(5)
                   .ValueGeneratedNever();

            builder.Property(u => u.IsDeleted)
                   .HasDefaultValue(false);

            builder.Property(u => u.CreationDate)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            // Name - Image
            builder.Property(x => x.Name)
                   .IsRequired()
                   .IsUnicode(true)
                   .HasMaxLength(70);

            builder.HasIndex(x => x.Name).IsUnique(false);

            builder.Property(x => x.Image)
                   .HasMaxLength(1000)
                   .IsRequired(false);

            // Relationships - 3
            builder.Property(x => x.GroupId).IsRequired();
            builder.Property(x => x.SubOneId).IsRequired();

            builder.HasOne<Group>()
                   .WithMany()
                   .HasForeignKey(x => x.SubOneId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<SubOne>()
                   .WithMany()
                   .HasForeignKey(x => x.SubOneId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany<Item>()
                   .WithOne()
                   .HasForeignKey(x => x.SubThreeId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
