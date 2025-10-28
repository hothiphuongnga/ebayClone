using AutoMapper;
using ebay.Dtos;
using ebay.Models;

public class ProductImageMapper : Profile
{
    public ProductImageMapper()
    {
        CreateMap<ProductImage, ProductImageDTO>().ReverseMap();
    }
}