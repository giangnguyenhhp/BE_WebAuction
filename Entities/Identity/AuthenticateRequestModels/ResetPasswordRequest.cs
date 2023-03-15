using System.ComponentModel.DataAnnotations;

namespace Entities.Identity.AuthenticateRequestModels;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
    public string? Password { get; set; }

    [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác")]
    public string? ConfirmPassword { get; set; }

    public string? Email { get; set; }
    public string? Token { get; set; }
}