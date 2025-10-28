namespace ebay.Dtos;
public class RatingDTO
{
    public int Id { get; set; }

    public int RaterId { get; set; }

    public int RatedUserId { get; set; }

    public int? ProductId { get; set; }

    public int RatingScore { get; set; }

    public string? Comment { get; set; }
    
}