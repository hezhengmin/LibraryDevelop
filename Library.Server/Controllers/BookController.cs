using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Zheng.Bll.Interfaces;
using Zheng.Mdl.Models.InputModel;
using Library.Server.Authorization;
using Library.Server.Filters;
using Library.Server.Services;

namespace Library.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ApiController
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService, ICurrentUser currentUser) 
            : base(currentUser)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// 取得圖書列表（分頁查詢）
        /// </summary>
        /// <param name="queryModel">查詢參數</param>
        /// <returns>圖書分頁資料</returns>
        [HttpPost("GetBooks")]
        [Permission(Permissions.Books.View)]
        public async Task<IActionResult> GetBooks([FromBody] BookQueryInputModel queryModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            try
            {
                var result = await _bookService.GetBooksAsync(
                    queryModel.PageNumber, 
                    queryModel.PageSize, 
                    queryModel.SearchTerm);
                return CustomResponse(result);
            }
            catch (Exception ex)
            {
                AddError($"取得圖書列表失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 根據ID取得圖書詳細資料
        /// </summary>
        /// <param name="id">圖書ID</param>
        /// <returns>圖書詳細資料</returns>
        [HttpGet("{id}")]
        [Permission(Permissions.Books.View)]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            try
            {
                var book = await _bookService.GetBookByIdAsync(id);
                
                if (book == null)
                {
                    AddError("找不到指定的圖書");
                    return CustomResponse();
                }

                return CustomResponse(book);
            }
            catch (Exception ex)
            {
                AddError($"取得圖書資料失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 新增圖書
        /// </summary>
        /// <param name="inputModel">圖書資料</param>
        /// <returns>新增的圖書資料</returns>
        [HttpPost]
        [Permission(Permissions.Books.Create)]
        public async Task<IActionResult> CreateBook([FromBody] BookInputModel inputModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            try
            {
                var book = await _bookService.CreateBookAsync(inputModel, CurrentUserId);
                return CustomResponse(book);
            }
            catch (Exception ex)
            {
                AddError($"新增圖書失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 更新圖書
        /// </summary>
        /// <param name="id">圖書ID</param>
        /// <param name="updateModel">更新資料</param>
        /// <returns>更新結果</returns>
        [HttpPut("{id}")]
        [Permission(Permissions.Books.Edit)]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookUpdateModel updateModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            try
            {
                var result = await _bookService.UpdateBookAsync(id, updateModel, CurrentUserId);

                if (!result)
                {
                    AddError("找不到指定的圖書或更新失敗");
                    return CustomResponse();
                }

                return CustomResponse("更新成功");
            }
            catch (Exception ex)
            {
                AddError($"更新圖書失敗：{ex.Message}");
                return CustomResponse();
            }
        }

        /// <summary>
        /// 刪除圖書
        /// </summary>
        /// <param name="id">圖書ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}")]
        [Permission(Permissions.Books.Delete)]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                var result = await _bookService.DeleteBookAsync(id);
                
                if (!result)
                {
                    AddError("找不到指定的圖書或刪除失敗");
                    return CustomResponse();
                }

                return CustomResponse("刪除成功");
            }
            catch (Exception ex)
            {
                AddError($"刪除圖書失敗：{ex.Message}");
                return CustomResponse();
            }
        }
    }
}
