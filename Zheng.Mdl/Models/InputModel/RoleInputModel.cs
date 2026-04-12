using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.InputModel
{
    public class RoleInputModel
    {
        [Required(ErrorMessage = "角色名稱為必填")]
        [StringLength(50, ErrorMessage = "角色名稱長度不能超過50個字元")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 角色權限列表
        /// </summary>
        public List<string>? Permissions { get; set; }
    }
}
