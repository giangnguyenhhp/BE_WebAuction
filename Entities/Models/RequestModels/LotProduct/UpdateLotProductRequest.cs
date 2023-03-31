namespace Entities.Models.RequestModels.LotProduct;

public class UpdateLotProductRequest
{

    public DateTime? TimeStarted { get; set; }

    public DateTime? TimeEnded { get; set; }
    public List<string>? ProductIds { get; set; }

}