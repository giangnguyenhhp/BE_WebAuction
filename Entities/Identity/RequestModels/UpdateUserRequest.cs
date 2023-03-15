using System.ComponentModel.DataAnnotations;

namespace Entities.Identity.RequestModels;

public class UpdateUserRequest
{
    [Required(ErrorMessage = "Yêu cầu nhập địa chỉ")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Yêu cầu nhập User Name")]
    public string? UserName { get; set; }
    

    public List<string>? RoleNames { get; set; }
}