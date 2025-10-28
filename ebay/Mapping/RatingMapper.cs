using AutoMapper;
using ebay.Dtos;
using ebay.Models;

public class RatingMapper : Profile
{
    public RatingMapper()
    {
        CreateMap<Rating, RatingDTO>().ReverseMap();
    }
}