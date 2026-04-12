using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Zheng.Infra.Data.Models;

[Table("RolePermission")]
public class RolePermission
{
    [Key]
    public int Id { get; set; }

    public Guid RoleId { get; set; }

    [Required]
    [StringLength(256)]
    public string Permission { get; set; } = null!;

    [ForeignKey("RoleId")]
    public virtual Role Role { get; set; } = null!;
}
