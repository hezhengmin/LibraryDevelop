using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl
{
    public enum StatusType : int
    {
        [Display(Name = "無庫存")]
        None = 0,
        [Display(Name = "有庫存")]
        Stock = 1
    }

    public enum TokenType
    {
        [Display(Name = "Refresh Token")]
        RefreshToken = 0,
        [Display(Name = "Email 驗證")]
        EmailVerification = 1,
        [Display(Name = "密碼重設")]
        PasswordReset = 2
    }
}
