using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zheng.Mdl.Models
{
    /// <summary>
    /// 分頁結果模型
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    public class PagedResult<T>
    {
        public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        /// <summary>
        /// 當前頁資料
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// 總筆數
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 當前頁碼
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 每頁筆數
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 總頁數
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 是否有上一頁
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// 是否有下一頁
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
