using AutoMapper;
using SControl.App.ViewModels;
using SControl.Business.Models;

namespace SControl.App.AutoMapper;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Curso, CursoViewModel>().ReverseMap();
    }
}