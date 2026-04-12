using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zheng.Bll.Interfaces;
using Zheng.Infra.Data.Models;
using Zheng.Mdl.Models;
using Zheng.Mdl.Models.Dto;
using Zheng.Mdl.Models.InputModel;

namespace Zheng.Bll.Services
{
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _bookRepository;

        public BookService(IGenericRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<PagedResult<BookDto>> GetBooksAsync(int pageNumber, int pageSize, string searchTerm = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // 限制最大每頁筆數

            var query = _bookRepository.GetAll();

            // 搜尋功能（可依書名、作者、出版社搜尋）
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(b => 
                    b.Title.Contains(searchTerm) || 
                    b.Creator.Contains(searchTerm) || 
                    b.Publisher.Contains(searchTerm));
            }

            // 取得總筆數
            var totalCount = await query.CountAsync();

            // 分頁並排序
            var books = await query
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Status = b.Status,
                    NumberOfCopies = b.NumberOfCopies,
                    ISBN = b.ISBN,
                    ISSN = b.ISSN,
                    GPN = b.GPN,
                    Publisher = b.Publisher,
                    Creator = b.Creator,
                    PublishDate = b.PublishDate,
                    Edition = b.Edition,
                    Cover = b.Cover,
                    Classify = b.Classify,
                    GPNType = b.GPNType,
                    Subject = b.Subject,
                    Language = b.Language,
                    Introduction = b.Introduction,
                    Price = b.Price,
                    TargetPeople = b.TargetPeople
                })
                .ToListAsync();

            return new PagedResult<BookDto>(books, totalCount, pageNumber, pageSize);
        }

        public async Task<BookDto> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetAll()
                .Where(b => b.Id == id)
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Status = b.Status,
                    NumberOfCopies = b.NumberOfCopies,
                    ISBN = b.ISBN,
                    ISSN = b.ISSN,
                    GPN = b.GPN,
                    Publisher = b.Publisher,
                    Creator = b.Creator,
                    PublishDate = b.PublishDate,
                    Edition = b.Edition,
                    Cover = b.Cover,
                    Classify = b.Classify,
                    GPNType = b.GPNType,
                    Subject = b.Subject,
                    Language = b.Language,
                    Introduction = b.Introduction,
                    Price = b.Price,
                    TargetPeople = b.TargetPeople
                })
                .FirstOrDefaultAsync();

            return book;
        }

        public async Task<BookDto> CreateBookAsync(BookInputModel inputModel, Guid userId)
        {
            var now = DateTime.Now;
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = inputModel.Title,
                Status = inputModel.Status,
                NumberOfCopies = inputModel.NumberOfCopies,
                ISBN = inputModel.ISBN,
                ISSN = inputModel.ISSN,
                GPN = inputModel.GPN,
                Publisher = inputModel.Publisher,
                RightCondition = inputModel.RightCondition,
                Creator = inputModel.Creator,
                PublishDate = inputModel.PublishDate,
                Edition = inputModel.Edition,
                Cover = inputModel.Cover,
                Classify = inputModel.Classify,
                GPNType = inputModel.GPNType,
                Subject = inputModel.Subject,
                Governance = inputModel.Governance,
                Grade = inputModel.Grade,
                Pages = inputModel.Pages,
                Size = inputModel.Size,
                Binding = inputModel.Binding,
                Language = inputModel.Language,
                Introduction = inputModel.Introduction,
                Catalog = inputModel.Catalog,
                Price = inputModel.Price,
                TargetPeople = inputModel.TargetPeople,
                Types = inputModel.Types,
                Attachment = inputModel.Attachment,
                Url = inputModel.Url,
                Duration = inputModel.Duration,
                Numbers = inputModel.Numbers,
                Restriction = inputModel.Restriction,
                CeasedDate = inputModel.CeasedDate,
                Authority = inputModel.Authority,
                CreatedAt = now,
                CreatedBy = userId,
                UpdatedAt = now,
                UpdatedBy = userId
            };

            _bookRepository.Add(book);

            return await GetBookByIdAsync(book.Id);
        }

        public async Task<bool> UpdateBookAsync(Guid id, BookUpdateModel updateModel, Guid userId)
        {
            var book = await _bookRepository.GetAll().FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
                return false;

            // 只更新有提供的欄位
            if (!string.IsNullOrWhiteSpace(updateModel.Title))
                book.Title = updateModel.Title;
            
            if (updateModel.Status.HasValue)
                book.Status = updateModel.Status.Value;
            
            if (updateModel.NumberOfCopies.HasValue)
                book.NumberOfCopies = updateModel.NumberOfCopies.Value;
            
            if (updateModel.ISBN != null)
                book.ISBN = updateModel.ISBN;
            
            if (updateModel.ISSN != null)
                book.ISSN = updateModel.ISSN;
            
            if (updateModel.GPN != null)
                book.GPN = updateModel.GPN;
            
            if (updateModel.Publisher != null)
                book.Publisher = updateModel.Publisher;
            
            if (updateModel.RightCondition != null)
                book.RightCondition = updateModel.RightCondition;
            
            if (updateModel.Creator != null)
                book.Creator = updateModel.Creator;
            
            if (updateModel.PublishDate.HasValue)
                book.PublishDate = updateModel.PublishDate.Value;
            
            if (updateModel.Edition != null)
                book.Edition = updateModel.Edition;
            
            if (updateModel.Cover != null)
                book.Cover = updateModel.Cover;
            
            if (updateModel.Classify != null)
                book.Classify = updateModel.Classify;
            
            if (updateModel.GPNType != null)
                book.GPNType = updateModel.GPNType;
            
            if (updateModel.Subject != null)
                book.Subject = updateModel.Subject;
            
            if (updateModel.Governance != null)
                book.Governance = updateModel.Governance;
            
            if (updateModel.Grade != null)
                book.Grade = updateModel.Grade;
            
            if (updateModel.Pages.HasValue)
                book.Pages = updateModel.Pages.Value;
            
            if (updateModel.Size != null)
                book.Size = updateModel.Size;
            
            if (updateModel.Binding != null)
                book.Binding = updateModel.Binding;
            
            if (updateModel.Language != null)
                book.Language = updateModel.Language;
            
            if (updateModel.Introduction != null)
                book.Introduction = updateModel.Introduction;
            
            if (updateModel.Catalog != null)
                book.Catalog = updateModel.Catalog;
            
            if (updateModel.Price.HasValue)
                book.Price = updateModel.Price.Value;
            
            if (updateModel.TargetPeople != null)
                book.TargetPeople = updateModel.TargetPeople;
            
            if (updateModel.Types != null)
                book.Types = updateModel.Types;
            
            if (updateModel.Attachment != null)
                book.Attachment = updateModel.Attachment;
            
            if (updateModel.Url != null)
                book.Url = updateModel.Url;
            
            if (updateModel.Duration != null)
                book.Duration = updateModel.Duration;
            
            if (updateModel.Numbers != null)
                book.Numbers = updateModel.Numbers;
            
            if (updateModel.Restriction != null)
                book.Restriction = updateModel.Restriction;
            
            if (updateModel.CeasedDate.HasValue)
                book.CeasedDate = updateModel.CeasedDate;
            
            if (updateModel.Authority != null)
                book.Authority = updateModel.Authority;

            book.UpdatedAt = DateTime.Now;
            book.UpdatedBy = userId;

            _bookRepository.Update(book);
            return true;
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var book = await _bookRepository.GetAll().FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
                return false;

            _bookRepository.Delete(book);
            return true;
        }
    }
}
