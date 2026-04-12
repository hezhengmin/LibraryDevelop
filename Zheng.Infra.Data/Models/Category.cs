using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Zheng.Mdl;


namespace Zheng.Infra.Data.Models;

[Table("Category")]
public partial class Category
{
    /// <summary>
    /// 系統編號
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    public Guid ParentId { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Title { get; set; }

    /// <summary>
    /// 新增時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 新增者
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// 修改時間
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 修改者
    /// </summary>
    public Guid UpdatedBy { get; set; }
}
