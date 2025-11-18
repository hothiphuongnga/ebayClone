namespace ebay.Dtos;

public class ProductDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; } // stock : tồn kho 

    public virtual ICollection<ProductImageDTO> ProductImages { get; set; } = new List<ProductImageDTO>();
    public virtual ICollection<RatingDTO> Ratings { get; set; } = new List<RatingDTO>();
} 
// thêm DTO -> nhớ thêm map 
public class ProductCreateDTO
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; } // stock : tồn kho 
}