using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Zheng.Mdl;


namespace Zheng.Infra.Data.Models;

[Table("BookPhoto")]
public partial class BookPhoto
{
    /// <summary>
    /// 系統編號
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 書籍Id
    /// </summary>
    public Guid BookId { get; set; }

    /// <summary>
    /// 檔案Id
    /// </summary>
    public Guid UploadFileId { get; set; }

    /// <summary>
    /// 系統日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime SystemDate { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("BookPhotos")]
    public virtual Book Book { get; set; }
}
