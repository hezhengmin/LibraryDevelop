using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Zheng.Mdl;


namespace Zheng.Infra.Data.Models;

[Table("SysMenu")]
public partial class SysMenu
{
    [Key]
    public long Id { get; set; }

    /// <summary>
    /// 父選單編號
    /// </summary>
    public long ParentModuleId { get; set; }

    /// <summary>
    /// 選單編號
    /// </summary>
    [Required]
    [StringLength(32)]
    public string ModuleNo { get; set; }

    /// <summary>
    /// 元件設定
    /// </summary>
    [StringLength(64)]
    public string Component { get; set; }

    /// <summary>
    /// 是否隱藏（1：是，0：否）
    /// </summary>
    public bool? Hidden { get; set; }

    /// <summary>
    /// 圖示
    /// </summary>
    [StringLength(32)]
    public string Icon { get; set; }

    /// <summary>
    /// 是否為選單（1：選單，0：按鈕）
    /// </summary>
    public bool IsMenu { get; set; }

    /// <summary>
    /// 是否預設展開（1：是，0：否）
    /// </summary>
    public bool? IsOpen { get; set; }

    /// <summary>
    /// 層級
    /// </summary>
    public int Levels { get; set; }

    /// <summary>
    /// 名稱
    /// </summary>
    [Required]
    [StringLength(64)]
    public string Name { get; set; }

    /// <summary>
    /// 排序順序
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 狀態（1：啟用，0：停用）
    /// </summary>
    public bool Status { get; set; }

    /// <summary>
    /// 滑鼠懸停提示文字
    /// </summary>
    [StringLength(32)]
    public string Tips { get; set; }

    /// <summary>
    /// 連結網址
    /// </summary>
    [StringLength(32)]
    public string Url { get; set; }

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
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 修改者
    /// </summary>
    public Guid? UpdatedBy { get; set; }
}
