using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zheng.Mdl.Models;
using Zheng.Mdl.Models.Dto;
using Zheng.Mdl.Models.InputModel;

namespace Zheng.Bll.Interfaces
{
    public interface IBookService
    {
        /// <summary>
        /// ���o�������y�M��
        /// </summary>
        /// <param name="pageNumber">���X</param>
        /// <param name="pageSize">�C������</param>
        /// <param name="searchTerm">�j�M����r�]�i��^</param>
        /// <returns>�������y�M��</returns>
        Task<PagedResult<BookDto>> GetBooksAsync(int pageNumber, int pageSize, string searchTerm = null);

        /// <summary>
        /// �ھ�ID���o���y
        /// </summary>
        /// <param name="id">���yID</param>
        /// <returns>���y���</returns>
        Task<BookDto> GetBookByIdAsync(Guid id);

        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <param name="inputModel">書籍輸入資料</param>
        /// <param name="userId">操作使用者ID</param>
        /// <returns>新增的書籍資料</returns>
        Task<BookDto> CreateBookAsync(BookInputModel inputModel, Guid userId);

        /// <summary>
        /// 更新書籍
        /// </summary>
        /// <param name="id">書籍ID</param>
        /// <param name="updateModel">書籍更新資料</param>
        /// <param name="userId">操作使用者ID</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateBookAsync(Guid id, BookUpdateModel updateModel, Guid userId);

        /// <summary>
        /// 刪除書籍
        /// </summary>
        /// <param name="id">書籍ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteBookAsync(Guid id);
    }
}
