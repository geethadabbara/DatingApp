using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, ListUserDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(
                    src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(
                    src => src.DateOfBirth.CalculateAge()
                ));
            CreateMap<User, DetailedUserDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(
                    src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(
                    src => src.DateOfBirth.CalculateAge()
                ));
            CreateMap<Photo, PhotosDto>();
            CreateMap<CreatePhotoDto, Photo>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<RegisterUserDto, User>();
            CreateMap<CreateMessageDto, Message>().ReverseMap();
            CreateMap<Message, MessageResponseDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));

        }
    }
}