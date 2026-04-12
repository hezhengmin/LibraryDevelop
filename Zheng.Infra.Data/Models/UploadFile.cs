using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Zheng.Mdl;


namespace Zheng.Infra.Data.Models;

[Table("UploadFile")]
public partial class UploadFile
{
    /// <summary>
    /// 系統編號
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 副檔名
    /// </summary>
    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Extension { get; set; }

    /// <summary>
    /// 檔案類型
    /// </summary>
    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string ContentType { get; set; }

    /// <summary>
    /// 檔案大小
    /// </summary>
    public long Length { get; set; }

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
