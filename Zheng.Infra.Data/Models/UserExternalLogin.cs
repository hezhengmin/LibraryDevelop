using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zheng.Infra.Data.Models;

[Table("UserExternalLogin")]
public class UserExternalLogin
{
    [Key]
    public int Id { get; set; }

    public Guid UserId { get; set; }

    [Required]
    [StringLength(128)]
    public string LoginProvider { get; set; } = null!;

    [Required]
    [StringLength(128)]
    public string ProviderKey { get; set; } = null!;

    [StringLength(256)]
    public string? ProviderDisplayName { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
