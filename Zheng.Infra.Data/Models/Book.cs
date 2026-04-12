using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Zheng.Mdl;


namespace Zheng.Infra.Data.Models;

[Table("Book")]
public partial class Book
{
    /// <summary>
    /// 系統編號
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// 書名標題
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Title { get; set; }

    /// <summary>
    /// 狀態(0有庫存1無庫存)
    /// </summary>
    public StatusType Status { get; set; }

    /// <summary>
    /// 庫存數量
    /// </summary>
    public int NumberOfCopies { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string ISBN { get; set; }

    [StringLength(8)]
    [Unicode(false)]
    public string ISSN { get; set; }

    /// <summary>
    /// GPN(政府出版品統一編號)
    /// </summary>
    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string GPN { get; set; }

    /// <summary>
    /// 出版單位
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Publisher { get; set; }

    /// <summary>
    /// 出版情況
    /// </summary>
    [Required]
    [StringLength(50)]
    public string RightCondition { get; set; }

    /// <summary>
    /// 作者資訊
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Creator { get; set; }

    /// <summary>
    /// 出版日期
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime PublishDate { get; set; }

    /// <summary>
    /// 版次
    /// </summary>
    [StringLength(50)]
    public string Edition { get; set; }

    /// <summary>
    /// 書封連結
    /// </summary>
    [StringLength(200)]
    public string Cover { get; set; }

    /// <summary>
    /// 書籍分類
    /// </summary>
    [StringLength(50)]
    public string Classify { get; set; }

    /// <summary>
    /// 出版品分類
    /// </summary>
    [Required]
    [StringLength(50)]
    public string GPNType { get; set; }

    /// <summary>
    /// 主題分類
    /// </summary>
    [StringLength(50)]
    public string Subject { get; set; }

    /// <summary>
    /// 施政分類
    /// </summary>
    [StringLength(50)]
    public string Governance { get; set; }

    /// <summary>
    /// 級別
    /// </summary>
    [StringLength(10)]
    public string Grade { get; set; }

    /// <summary>
    /// 頁數
    /// </summary>
    public int Pages { get; set; }

    /// <summary>
    /// 開數
    /// </summary>
    [StringLength(50)]
    public string Size { get; set; }

    /// <summary>
    /// 裝訂
    /// </summary>
    [StringLength(10)]
    public string Binding { get; set; }

    /// <summary>
    /// 語言
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Language { get; set; }

    /// <summary>
    /// abstract(書籍介紹)
    /// </summary>
    [StringLength(4000)]
    public string Introduction { get; set; }

    /// <summary>
    /// 目次
    /// </summary>
    [StringLength(4000)]
    public string Catalog { get; set; }

    /// <summary>
    /// 價格
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// 適用對象
    /// </summary>
    [StringLength(50)]
    public string TargetPeople { get; set; }

    /// <summary>
    /// 資料類型
    /// </summary>
    [StringLength(50)]
    public string Types { get; set; }

    /// <summary>
    /// 附件
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Attachment { get; set; }

    /// <summary>
    /// 出版品網址-線上版或試閱版
    /// </summary>
    [StringLength(1000)]
    [Unicode(false)]
    public string Url { get; set; }

    /// <summary>
    /// 播放時間長度
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Duration { get; set; }

    /// <summary>
    /// 字號
    /// </summary>
    [StringLength(50)]
    public string Numbers { get; set; }

    /// <summary>
    /// 權利範圍
    /// </summary>
    [StringLength(10)]
    public string Restriction { get; set; }

    /// <summary>
    /// 停刊註記
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? CeasedDate { get; set; }

    /// <summary>
    /// 授權資訊
    /// </summary>
    [Required]
    [StringLength(500)]
    public string Authority { get; set; }

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

    [InverseProperty("Book")]
    public virtual ICollection<BookPhoto> BookPhotos { get; set; } = new List<BookPhoto>();

    [InverseProperty("Book")]
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
