using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Zheng.Mdl;


namespace Zheng.Infra.Data.Models;

[Table("Loan")]
public partial class Loan
{
    /// <summary>
    /// 系統編號
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BookId { get; set; }

    /// <summary>
    /// 借出開始日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime IssueDate { get; set; }

    /// <summary>
    /// 借出結束日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime DueDate { get; set; }

    /// <summary>
    /// 書籍歸還日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? ReturnDate { get; set; }

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

    [ForeignKey("UserId")]
    [InverseProperty("Loans")]
    public virtual User User { get; set; }

    [ForeignKey("BookId")]
    [InverseProperty("Loans")]
    public virtual Book Book { get; set; }
}
