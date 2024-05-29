﻿using AutoMapper;
using PersonRegistrationSystem.Common.DTOs;
using PersonRegistrationSystem.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<UserRegisterDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Salt, opt => opt.Ignore())
                .ForMember(dest => dest.Persons, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "User"))
                .ReverseMap();

            CreateMap<UserLoginDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.Salt, opt => opt.Ignore())
                .ForMember(dest => dest.Persons, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<PersonDTO, Person>().ReverseMap();
            CreateMap<PersonCreateDTO, Person>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<PersonUpdateDTO, Person>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.PlaceOfResidence, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<PlaceOfResidenceDTO, PlaceOfResidence>().ReverseMap();
            CreateMap<PlaceOfResidenceUpdateDTO, PlaceOfResidence>()
                .ForMember(dest => dest.Person, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}