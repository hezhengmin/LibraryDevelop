using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Zheng.Infra.Data.Models;

namespace Zheng.Infra.Data.Models.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasIndex(rp => new { rp.RoleId, rp.Permission }, "IX_RolePermission_RoleId_Permission")
            .IsUnique();

        builder.HasIndex(rp => rp.RoleId, "IX_RolePermission_RoleId");
    }
}
