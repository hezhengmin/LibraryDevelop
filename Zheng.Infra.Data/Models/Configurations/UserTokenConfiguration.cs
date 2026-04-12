using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zheng.Infra.Data.Models;

namespace Zheng.Infra.Data.Models.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.HasIndex(ut => ut.TokenValue, "IX_UserToken_TokenValue");

        builder.HasIndex(ut => new { ut.UserId, ut.TokenType }, "IX_UserToken_UserId_TokenType");

        builder.HasIndex(ut => ut.UserId, "IX_UserToken_UserId");

        builder.Property(ut => ut.ExpiryDate).HasColumnType("datetime2");
        builder.Property(ut => ut.IsUsed).HasDefaultValue(false);
        builder.Property(ut => ut.IsRevoked).HasDefaultValue(false);
        builder.Property(ut => ut.CreatedAt).HasColumnType("datetime2");
    }
}
