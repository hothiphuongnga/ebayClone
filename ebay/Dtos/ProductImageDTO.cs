namespace ebay.Dtos;

public class ProductImageDTO
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsPrimary { get; set; }
}

public class ProductImage_ProductDTO
{
    public int Id { get; set; }

    public int? ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsPrimary { get; set; }
    public ProductDTO? Product { get; set; }
}