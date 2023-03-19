using System.ComponentModel.DataAnnotations;

namespace Entities.Models.RequestModels.Product;

public class CreateProductRequest
{
    
    [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
    public string? ProductName { get; set; }
    [Required(ErrorMessage = "Yêu cầu nhập giá khởi điểm")]
    public double PriceOpen { get; set; }
    [Required(ErrorMessage = "Yêu cầu nhập thông tin sản phẩm")]
    public string? Description { get; set; }

    public bool IsApproved { get; set; }
}