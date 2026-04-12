using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zheng.Mdl.Models.Dto
{
    /// <summary>
    /// 書籍資料傳輸物件
    /// </summary>
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public StatusType Status { get; set; }
        public int NumberOfCopies { get; set; }
        public string ISBN { get; set; }
        public string ISSN { get; set; }
        public string GPN { get; set; }
        public string Publisher { get; set; }
        public string Creator { get; set; }
        public DateTime PublishDate { get; set; }
        public string Edition { get; set; }
        public string Cover { get; set; }
        public string Classify { get; set; }
        public string GPNType { get; set; }
        public string Subject { get; set; }
        public string Language { get; set; }
        public string Introduction { get; set; }
        public int Price { get; set; }
        public string TargetPeople { get; set; }
    }
}
