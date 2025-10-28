using AutoMapper;
using ebay.Dtos;
using ebay.Models;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<Product, ProductDTO>().ReverseMap();
    }
}