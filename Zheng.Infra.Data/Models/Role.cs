using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zheng.Infra.Data.Models;

[Table("Role")]
public class Role
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(256)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(256)]
    public string NormalizedName { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? ConcurrencyStamp { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
