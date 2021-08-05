using AutoMapper;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using EngelsizHayatAuthServer.Core.Dtos.CommentDtos;
using EngelsizHayatAuthServer.Core.Dtos.LocationDtos;
using EngelsizHayatAuthServer.Core.Dtos.MessageDtos;
using EngelsizHayatAuthServer.Core.Dtos.NestedCommentDtos;
using EngelsizHayatAuthServer.Core.Dtos.PostDtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Dtos.Util;

namespace EngelsizHayatAuthServer.Service
{
    class DtoMapper : Profile
    {
        public DtoMapper()
        {

            CreateMap<UserAppDto, UserApp>().ReverseMap();
            CreateMap<UserAppLikeDto, UserApp>().ReverseMap();
            CreateMap<UserNameAndPhotoPathDto, UserApp>().ReverseMap();
            CreateMap<UserSearchDto, UserApp>().ReverseMap();

            CreateMap<AddCommentDto, Comment>().ReverseMap();
            CreateMap<DeleteCommentDto, Comment>().ReverseMap();
            CreateMap<UpdateCommentDto, Comment>().ReverseMap();
            CreateMap<CommentDto, Comment>().ReverseMap();

            CreateMap<AddNestedCommentDto, NestedComment>().ReverseMap();
            CreateMap<UpdateNestedCommentDto, NestedComment>().ReverseMap();
            CreateMap<DeleteNestedCommentDto, NestedComment>().ReverseMap();

            CreateMap<AddPostDto, Post>().ReverseMap();
            CreateMap<DeletePostDto, Post>().ReverseMap();
            CreateMap<UpdatePostDto, Post>().ReverseMap();
            CreateMap<PostActivityDto, Post>().ReverseMap();

            CreateMap<UpdateLocationDto, Location>().ReverseMap();

            CreateMap<LikeDto, Like>().ReverseMap();
            CreateMap<LikeListDto, Like>().ReverseMap();
        }
    }
}
