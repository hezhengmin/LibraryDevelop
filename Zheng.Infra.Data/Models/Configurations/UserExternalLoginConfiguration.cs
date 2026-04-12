using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zheng.Infra.Data.Models;

namespace Zheng.Infra.Data.Models.Configurations;

public class UserExternalLoginConfiguration : IEntityTypeConfiguration<UserExternalLogin>
{
    public void Configure(EntityTypeBuilder<UserExternalLogin> builder)
    {
        builder.HasIndex(uel => new { uel.LoginProvider, uel.ProviderKey }, "IX_UserExternalLogin_LoginProvider_ProviderKey")
            .IsUnique();

        builder.HasIndex(uel => uel.UserId, "IX_UserExternalLogin_UserId");

        builder.Property(uel => uel.CreatedAt).HasColumnType("datetime2");
    }
}
