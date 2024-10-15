using AutoMapper;
using CrudDapper.Dto;
using CrudDapper.Models;

namespace CrudDapper.Profiles
{
    public class ProfileAutorMapper : Profile
    {
        public ProfileAutorMapper()
        {
            CreateMap<Usuario, UsuarioListarDto>();

        }
    }
}
