using AutoMapper;
using FeedbackV1.Dtos;
using FeedbackV1.Models;

namespace FeedbackV1.Helpers

{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
          CreateMap<Feedbacks, FeedbackListDto>(); 
          CreateMap<Employees, EmployeeListDto>(); 
          CreateMap<RequestSendDto, Feedbacks>();
          CreateMap<RequestFeedbackDto, Feedbacks>();
          CreateMap<User, UserDto>();
          CreateMap<Departament, DepartamentListDto>();
          CreateMap<UserForRegisterDto, User>();
          CreateMap<UpdateUserDto, User>();
          CreateMap<UserDto, User>();
          CreateMap<GiveFeedbackDto, Feedbacks>();
        }
    }
}