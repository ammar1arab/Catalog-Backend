using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.EntityConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t => t.HasCheckConstraint("CH_Name_Length", "LEN(Username) >= 3"));

        // Id - IsDeleted - CreationDate
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(u => u.IsDeleted).HasDefaultValue(false);
        builder.Property(u => u.CreationDate).IsRequired().HasDefaultValueSql("GETDATE()");


        // Username
        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.Username).IsUnique();


        // PasswordHash
        builder.Property(u => u.PasswordHash).IsRequired();


        // Role
        builder.Property(u => u.Role).IsRequired().HasMaxLength(20).HasDefaultValue("Admin");
    }
}