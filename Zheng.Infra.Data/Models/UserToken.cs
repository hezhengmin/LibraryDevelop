using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Zheng.Mdl;

namespace Zheng.Infra.Data.Models;

[Table("UserToken")]
public class UserToken
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string TokenType { get; set; } = null!;

    [Required]
    [StringLength(500)]
    public string TokenValue { get; set; } = null!;

    public string? AccessToken { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
