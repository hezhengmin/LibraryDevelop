using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.Attributes
{
    /// <summary>
    /// 選填的電子郵件驗證屬性 - 允許 null 或空白值
    /// </summary>
    public class OptionalEmailAttribute : ValidationAttribute
    {
        private readonly EmailAddressAttribute _emailValidator = new EmailAddressAttribute();

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // 如果值為 null 或空白字串，則視為有效
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            // 否則使用 EmailAddressAttribute 的驗證邏輯
            if (_emailValidator.IsValid(value))
            {
                return ValidationResult.Success;
            }

            // 驗證失敗，返回錯誤訊息
            return new ValidationResult(ErrorMessage ?? "電子郵件格式不正確");
        }
    }
}
