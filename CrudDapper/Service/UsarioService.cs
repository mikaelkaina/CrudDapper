﻿using AutoMapper;
using CrudDapper.Dto;
using CrudDapper.Models;
using Dapper;
using System.Data.SqlClient;

namespace CrudDapper.Service
{
    public class UsarioService : IUsuarioInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UsarioService(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseModel<UsuarioListarDto>> BuscarUsuarioPorId(int usuarioId)
        {
            ResponseModel<UsuarioListarDto> response = new ResponseModel<UsuarioListarDto>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuarioBanco = await connection.QueryFirstOrDefaultAsync<Usuario>("select * from Usuarios where id = @Id", new {Id = usuarioId});
                
                if (usuarioBanco == null)
                {
                    response.Mensagem = "Nenhum usuário localizado.";
                    response.Status = false;
                    return response;
                }

                var usuarioMapeado = _mapper.Map<UsuarioListarDto>(usuarioBanco);

                response.Dados = usuarioMapeado;
                response.Mensagem = "Usuário localizado com sucesso.";
            }
            return response;
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> BuscarUsusario()
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuariosBanco = await connection.QueryAsync<Usuario>("select * from Usuarios");
                if (usuariosBanco.Count() == 0)
                {
                    response.Mensagem = "Nenhum usuário localizado.";
                    response.Status = false;
                    return response;
                }

                var usurioMapeado = _mapper.Map<List<UsuarioListarDto>>(usuariosBanco);

                response.Dados = usurioMapeado;
                response.Mensagem = "Usuários localizados com sucesso.";
            }
            return response;
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> CriarUsuario(UsuarioCriarDto usuarioCriarDto)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuariosBanco = await connection.ExecuteAsync("insert into Usuarios (NomeCompleto, Email, Cargo, Salario, CPF, Senha, Situacao) " +
                                                   "values (@NomeCompleto, @Email, @Cargo, @Salario, @CPF, @Senha, @Situacao)", usuarioCriarDto);

                if(usuariosBanco == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar o resgistro.";
                    response.Status = false;
                    return response;
                }

                var usuarios = await ListarUsuarios(connection);
                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);

                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários listados com sucesso";
            }
            return response;
        }

        private static async Task<IEnumerable<Usuario>> ListarUsuarios(SqlConnection connection)
        {
            return await connection.QueryAsync<Usuario>("select * from Usuarios ");
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> EditarUsuario(UsuarioEditarDto usuarioEditarDto)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuariosBanco = await connection.ExecuteAsync("update Usuarios set NomeCompleto = @NomeCompleto," +
                                                                   "Email = @Email, Cargo = @Cargo, Salario = @Salario, situacao" +
                                                                   "= @Situacao, CPF = @CPF where Id = @Id", usuarioEditarDto);

                if (usuariosBanco == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar a edição.";
                    response.Status = false;
                    return response;
                }

                var usuarios = await ListarUsuarios(connection);
                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);
                
                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários listados com sucesso";
            }
            return response;
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> RemoverUsuario(int usuarioId)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuariosBanco = await connection.ExecuteAsync("delete from Usuarios where id = @Id", new { Id = usuarioId });

                if (usuariosBanco == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao tentar remover usuário.";
                    response.Status = false;
                    return response;
                }

                var usuarios = await ListarUsuarios(connection);
                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);
                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários listados com sucesso";
            }
            return response;
        }
    }
}
