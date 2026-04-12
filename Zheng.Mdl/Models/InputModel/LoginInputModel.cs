using System.ComponentModel.DataAnnotations;

namespace Zheng.Mdl.Models.InputModel
{
    public class LoginInputModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }
}