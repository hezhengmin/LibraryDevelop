using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zheng.Infra.Data.Models;

[Table("User")]
public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = null!;

    [Required]
    [StringLength(256)]
    public string NormalizedUserName { get; set; } = null!;

    [Required]
    [StringLength(256)]
    public string Email { get; set; } = null!;

    [Required]
    [StringLength(256)]
    public string NormalizedEmail { get; set; } = null!;

    public bool EmailConfirmed { get; set; }

    [StringLength(500)]
    public string? PasswordHash { get; set; }

    public int PasswordHashVersion { get; set; }

    [StringLength(100)]
    public string? SecurityStamp { get; set; }

    [StringLength(50)]
    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [StringLength(100)]
    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<UserExternalLogin> UserExternalLogins { get; set; } = new List<UserExternalLogin>();
    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
