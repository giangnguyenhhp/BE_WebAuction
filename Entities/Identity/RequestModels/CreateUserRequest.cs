using System.ComponentModel.DataAnnotations;

namespace Entities.Identity.RequestModels;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Yêu cầu nhập địa chỉ")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "Yêu cầu nhập Email")]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required(ErrorMessage="Yêu cầu nhập tên đăng nhập")]
    public string? UserName { get; set; }

    [Required(ErrorMessage="Yêu cầu nhập mật khẩu")]
    public string? Password { get; set; }

    [Required(ErrorMessage="Yêu cầu nhập số điện thoại")]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Compare("Password", ErrorMessage="Mật khẩu lặp lại không chính xác")]
    public string? ConfirmPassword { get; set; }

    public List<string>? RoleNames { get; set; }
}