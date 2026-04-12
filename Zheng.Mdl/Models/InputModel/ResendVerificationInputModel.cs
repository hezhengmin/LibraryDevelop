using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.InputModel
{
    public class ResendVerificationInputModel
    {
        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
        public string Email { get; set; }
    }
}
