namespace Entities.Models.RequestModels.LotProduct;

public class CreateLotProductRequest
{
    public DateTime? TimeStarted { get; set; }

    public DateTime? TimeEnded { get; set; }
    public List<string>? ProductIds { get; set; }
}