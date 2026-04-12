using System.ComponentModel.DataAnnotations;
using Zheng.Mdl.Models.Attributes;

namespace Zheng.Mdl.Models.InputModel
{
    /// <summary>
    /// 使用者新增輸入模型
    /// </summary>
    public class UserInputModel
    {
        [Required(ErrorMessage = "使用者名稱為必填")]
        [StringLength(50, ErrorMessage = "使用者名稱長度不能超過50個字元")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "密碼為必填")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度應在6到100個字元之間")]
        public string Password { get; set; } = string.Empty;

        [OptionalEmail(ErrorMessage = "電子郵件格式不正確")]
        public string? Email { get; set; } = null;

        [OptionalPhone(ErrorMessage = "電話號碼格式不正確")]
        public string? PhoneNumber { get; set; } = null;

        public bool EmailConfirmed { get; set; } = false;

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<string>? Roles { get; set; }
    }

    /// <summary>
    /// 使用者更新輸入模型
    /// </summary>
    public class UserUpdateInputModel
    {
        [OptionalEmail(ErrorMessage = "電子郵件格式不正確")]
        public string? Email { get; set; }

        [OptionalPhone(ErrorMessage = "電話號碼格式不正確")]
        public string? PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; } = false;

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<string>? Roles { get; set; }
    }

    /// <summary>
    /// 重設密碼輸入模型
    /// </summary>
    public class ResetPasswordInputModel
    {
        [Required(ErrorMessage = "新密碼為必填")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "密碼長度應在6到100個字元之間")]
        public string NewPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// 設定角色輸入模型
    /// </summary>
    public class SetRolesInputModel
    {
        [Required(ErrorMessage = "角色列表為必填")]
        public List<string> Roles { get; set; } = new List<string>();
    }
}
