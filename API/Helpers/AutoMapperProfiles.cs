using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extentions;
using AutoMapper;

namespace API.Helpers
{
    //Maps from an Object to Another ====> a = b / a and b are not from the same class
    public class AutoMapperProfiles :Profile
    {
        public AutoMapperProfiles(){
            
            CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(img => img.IsMain).Url))
            .ForMember(dest => dest.Age, opt =>opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDto>();
        }
        
    }
}