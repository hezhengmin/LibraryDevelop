using System;
using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.InputModel
{
    /// <summary>
    /// 系統選單新增輸入模型
    /// </summary>
    public class SysMenuCreateInputModel
    {
        /// <summary>
        /// 父選單編號
        /// </summary>
        [Required(ErrorMessage = "父選單編號為必填")]
        public long ParentModuleId { get; set; }

        /// <summary>
        /// 選單編號
        /// </summary>
        [Required(ErrorMessage = "選單編號為必填")]
        [StringLength(32, ErrorMessage = "選單編號長度不可超過32個字元")]
        public string ModuleNo { get; set; }

        /// <summary>
        /// 元件設定
        /// </summary>
        [StringLength(64, ErrorMessage = "元件設定長度不可超過64個字元")]
        public string Component { get; set; }

        /// <summary>
        /// 是否隱藏（1：是，0：否）
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// 圖示
        /// </summary>
        [StringLength(32, ErrorMessage = "圖示長度不可超過32個字元")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否為選單（1：選單，0：按鈕）
        /// </summary>
        [Required(ErrorMessage = "是否為選單為必填")]
        public bool IsMenu { get; set; }

        /// <summary>
        /// 是否預設展開（1：是，0：否）
        /// </summary>
        public bool? IsOpen { get; set; }

        /// <summary>
        /// 層級
        /// </summary>
        [Required(ErrorMessage = "層級為必填")]
        [Range(1, 10, ErrorMessage = "層級必須介於1到10之間")]
        public int Levels { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Required(ErrorMessage = "名稱為必填")]
        [StringLength(64, ErrorMessage = "名稱長度不可超過64個字元")]
        public string Name { get; set; }

        /// <summary>
        /// 排序順序
        /// </summary>
        [Required(ErrorMessage = "排序順序為必填")]
        public int SortOrder { get; set; }

        /// <summary>
        /// 狀態（1：啟用，0：停用）
        /// </summary>
        [Required(ErrorMessage = "狀態為必填")]
        public bool Status { get; set; }

        /// <summary>
        /// 滑鼠懸停提示文字
        /// </summary>
        [StringLength(32, ErrorMessage = "提示文字長度不可超過32個字元")]
        public string Tips { get; set; }

        /// <summary>
        /// 連結網址
        /// </summary>
        [StringLength(32, ErrorMessage = "連結網址長度不可超過32個字元")]
        public string Url { get; set; }
    }

    /// <summary>
    /// 系統選單更新輸入模型
    /// </summary>
    public class SysMenuUpdateInputModel
    {
        /// <summary>
        /// 父選單編號
        /// </summary>
        public long? ParentModuleId { get; set; }

        /// <summary>
        /// 選單編號
        /// </summary>
        [StringLength(32, ErrorMessage = "選單編號長度不可超過32個字元")]
        public string ModuleNo { get; set; }

        /// <summary>
        /// 元件設定
        /// </summary>
        [StringLength(64, ErrorMessage = "元件設定長度不可超過64個字元")]
        public string Component { get; set; }

        /// <summary>
        /// 是否隱藏（1：是，0：否）
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// 圖示
        /// </summary>
        [StringLength(32, ErrorMessage = "圖示長度不可超過32個字元")]
        public string Icon { get; set; }

        /// <summary>
        /// 是否為選單（1：選單，0：按鈕）
        /// </summary>
        public bool? IsMenu { get; set; }

        /// <summary>
        /// 是否預設展開（1：是，0：否）
        /// </summary>
        public bool? IsOpen { get; set; }

        /// <summary>
        /// 層級
        /// </summary>
        [Range(1, 10, ErrorMessage = "層級必須介於1到10之間")]
        public int? Levels { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [StringLength(64, ErrorMessage = "名稱長度不可超過64個字元")]
        public string Name { get; set; }

        /// <summary>
        /// 排序順序
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// 狀態（1：啟用，0：停用）
        /// </summary>
        public bool? Status { get; set; }

        /// <summary>
        /// 滑鼠懸停提示文字
        /// </summary>
        [StringLength(32, ErrorMessage = "提示文字長度不可超過32個字元")]
        public string Tips { get; set; }

        /// <summary>
        /// 連結網址
        /// </summary>
        [StringLength(32, ErrorMessage = "連結網址長度不可超過32個字元")]
        public string Url { get; set; }
    }
}
