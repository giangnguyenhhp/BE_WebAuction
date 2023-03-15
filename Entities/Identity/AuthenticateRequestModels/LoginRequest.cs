using System.ComponentModel.DataAnnotations;

namespace Entities.Identity.AuthenticateRequestModels;

public class LoginRequest
{
    [Required(ErrorMessage = "Yêu cầu nhập UserName hoặc Email")]
    public string? UserNameOrEmail { get; set; }
    [Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
    public string? Password { get; set; }
}