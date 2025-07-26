using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.EntityConfiguration
{
    public class SubOneConfiguration : IEntityTypeConfiguration<SubOne>
    {
        public void Configure(EntityTypeBuilder<SubOne> builder)
        {
            builder.ToTable("SubOnes");

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

            // Relationship - 1
            builder.HasMany<SubTwo>()
                   .WithOne()
                   .HasForeignKey(x => x.SubOneId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
