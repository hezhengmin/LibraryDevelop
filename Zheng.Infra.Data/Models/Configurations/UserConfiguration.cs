using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zheng.Infra.Data.Models;

namespace Zheng.Infra.Data.Models.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.NormalizedUserName, "IX_User_NormalizedUserName")
            .IsUnique();

        builder.HasIndex(u => u.NormalizedEmail, "IX_User_NormalizedEmail")
            .IsUnique();

        builder.Property(u => u.EmailConfirmed).HasDefaultValue(false);
        builder.Property(u => u.PasswordHashVersion).HasDefaultValue(2);
        builder.Property(u => u.PhoneNumberConfirmed).HasDefaultValue(false);
        builder.Property(u => u.TwoFactorEnabled).HasDefaultValue(false);
        builder.Property(u => u.LockoutEnabled).HasDefaultValue(true);
        builder.Property(u => u.AccessFailedCount).HasDefaultValue(0);
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).HasColumnType("datetime2");
        builder.Property(u => u.UpdatedAt).HasColumnType("datetime2");

        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserExternalLogins)
            .WithOne(uel => uel.User)
            .HasForeignKey(uel => uel.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.UserTokens)
            .WithOne(ut => ut.User)
            .HasForeignKey(ut => ut.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
