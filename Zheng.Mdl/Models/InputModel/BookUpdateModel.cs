using System;
using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.InputModel
{
    /// <summary>
    /// 書籍更新輸入模型
    /// </summary>
    public class BookUpdateModel
    {
        /// <summary>
        /// 書名標題
        /// </summary>
        [StringLength(150, ErrorMessage = "書名長度不能超過150個字元")]
        public string Title { get; set; }

        /// <summary>
        /// 狀態(0無庫存1有庫存)
        /// </summary>
        public StatusType? Status { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "庫存數量不能為負數")]
        public int? NumberOfCopies { get; set; }

        /// <summary>
        /// ISBN
        /// </summary>
        [StringLength(15, ErrorMessage = "ISBN長度不能超過15個字元")]
        public string ISBN { get; set; }

        /// <summary>
        /// ISSN
        /// </summary>
        [StringLength(8, ErrorMessage = "ISSN長度不能超過8個字元")]
        public string ISSN { get; set; }

        /// <summary>
        /// GPN(政府出版品統一編號)
        /// </summary>
        [StringLength(10, ErrorMessage = "GPN長度不能超過10個字元")]
        public string GPN { get; set; }

        /// <summary>
        /// 出版單位
        /// </summary>
        [StringLength(50, ErrorMessage = "出版單位長度不能超過50個字元")]
        public string Publisher { get; set; }

        /// <summary>
        /// 出版情況
        /// </summary>
        [StringLength(50, ErrorMessage = "出版情況長度不能超過50個字元")]
        public string RightCondition { get; set; }

        /// <summary>
        /// 作者資訊
        /// </summary>
        [StringLength(500, ErrorMessage = "作者資訊長度不能超過500個字元")]
        public string Creator { get; set; }

        /// <summary>
        /// 出版日期
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// 版次
        /// </summary>
        [StringLength(50, ErrorMessage = "版次長度不能超過50個字元")]
        public string Edition { get; set; }

        /// <summary>
        /// 書封連結
        /// </summary>
        [StringLength(200, ErrorMessage = "書封連結長度不能超過200個字元")]
        public string Cover { get; set; }

        /// <summary>
        /// 書籍分類
        /// </summary>
        [StringLength(50, ErrorMessage = "書籍分類長度不能超過50個字元")]
        public string Classify { get; set; }

        /// <summary>
        /// 出版品分類
        /// </summary>
        [StringLength(50, ErrorMessage = "出版品分類長度不能超過50個字元")]
        public string GPNType { get; set; }

        /// <summary>
        /// 主題分類
        /// </summary>
        [StringLength(50, ErrorMessage = "主題分類長度不能超過50個字元")]
        public string Subject { get; set; }

        /// <summary>
        /// 施政分類
        /// </summary>
        [StringLength(50, ErrorMessage = "施政分類長度不能超過50個字元")]
        public string Governance { get; set; }

        /// <summary>
        /// 級別
        /// </summary>
        [StringLength(10, ErrorMessage = "級別長度不能超過10個字元")]
        public string Grade { get; set; }

        /// <summary>
        /// 頁數
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "頁數不能為負數")]
        public int? Pages { get; set; }

        /// <summary>
        /// 開數
        /// </summary>
        [StringLength(50, ErrorMessage = "開數長度不能超過50個字元")]
        public string Size { get; set; }

        /// <summary>
        /// 裝訂
        /// </summary>
        [StringLength(10, ErrorMessage = "裝訂長度不能超過10個字元")]
        public string Binding { get; set; }

        /// <summary>
        /// 語言
        /// </summary>
        [StringLength(10, ErrorMessage = "語言長度不能超過10個字元")]
        public string Language { get; set; }

        /// <summary>
        /// 書籍介紹
        /// </summary>
        [StringLength(4000, ErrorMessage = "書籍介紹長度不能超過4000個字元")]
        public string Introduction { get; set; }

        /// <summary>
        /// 目次
        /// </summary>
        [StringLength(4000, ErrorMessage = "目次長度不能超過4000個字元")]
        public string Catalog { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "價格不能為負數")]
        public int? Price { get; set; }

        /// <summary>
        /// 適用對象
        /// </summary>
        [StringLength(50, ErrorMessage = "適用對象長度不能超過50個字元")]
        public string TargetPeople { get; set; }

        /// <summary>
        /// 資料類型
        /// </summary>
        [StringLength(50, ErrorMessage = "資料類型長度不能超過50個字元")]
        public string Types { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        [StringLength(50, ErrorMessage = "附件長度不能超過50個字元")]
        public string Attachment { get; set; }

        /// <summary>
        /// 出版品網址
        /// </summary>
        [StringLength(1000, ErrorMessage = "出版品網址長度不能超過1000個字元")]
        public string Url { get; set; }

        /// <summary>
        /// 播放時間長度
        /// </summary>
        [StringLength(50, ErrorMessage = "播放時間長度不能超過50個字元")]
        public string Duration { get; set; }

        /// <summary>
        /// 字號
        /// </summary>
        [StringLength(50, ErrorMessage = "字號長度不能超過50個字元")]
        public string Numbers { get; set; }

        /// <summary>
        /// 權利範圍
        /// </summary>
        [StringLength(10, ErrorMessage = "權利範圍長度不能超過10個字元")]
        public string Restriction { get; set; }

        /// <summary>
        /// 停刊註記
        /// </summary>
        public DateTime? CeasedDate { get; set; }

        /// <summary>
        /// 授權資訊
        /// </summary>
        [StringLength(500, ErrorMessage = "授權資訊長度不能超過500個字元")]
        public string Authority { get; set; }
    }
}
