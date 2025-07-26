using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.EntityConfiguration
{
    public class SubTwoConfiguration : IEntityTypeConfiguration<SubTwo>
    {
        public void Configure(EntityTypeBuilder<SubTwo> builder)
        {
            builder.ToTable("SubTwos");

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

            builder.HasOne<Group>()
                   .WithMany()
                   .HasForeignKey(x => x.SubOneId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany<SubThree>()
                   .WithOne()
                   .HasForeignKey(x => x.SubTwoId);

            builder.HasMany<Item>()
                   .WithOne()
                   .HasForeignKey(x => x.SubTwoId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
