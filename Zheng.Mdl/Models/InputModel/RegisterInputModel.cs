using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.InputModel
{
    public class RegisterInputModel
    {
        [Required(ErrorMessage = "請輸入帳號")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "請輸入電子郵件")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
        public string Email { get; set; }

        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度須介於6~100字元")]
        public string Password { get; set; }

        [Required(ErrorMessage = "請確認密碼")]
        [Compare("Password", ErrorMessage = "密碼與確認密碼不一致")]
        public string ConfirmPassword { get; set; }
    }
}