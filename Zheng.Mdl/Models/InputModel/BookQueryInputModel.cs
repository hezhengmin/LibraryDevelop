using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.InputModel
{
    /// <summary>
    /// 書籍查詢輸入模型
    /// </summary>
    public class BookQueryInputModel
    {
        /// <summary>
        /// 頁碼（預設為1）
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "頁碼必須大於0")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// 每頁筆數（預設為10）
        /// </summary>
        [Range(1, 100, ErrorMessage = "每頁筆數必須介於1到100之間")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 搜尋關鍵字（可選）
        /// </summary>
        public string SearchTerm { get; set; }
    }
}
