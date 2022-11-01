using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace ManejoPresupuesto.Services {
    public class UsuarioRepository : IUsuarioRepository {
        private readonly string configurationString;
        private readonly HttpContext httpContext;

        public UsuarioRepository(IConfiguration configuration,
                                 IHttpContextAccessor httpContextAccessor) {
            configurationString = configuration.GetConnectionString("DefaultConnection");
            httpContext = httpContextAccessor.HttpContext;
        }

        public int ObtenerUsuarioID() {
            if (httpContext.User.Identity.IsAuthenticated) { 
                var idClaim = httpContext.User.Claims
                                              .Where(x => x.Type == ClaimTypes.NameIdentifier) 
                                              .FirstOrDefault();

                /* Se obtiene el id del usuario */
                var id = int.Parse(idClaim.Value);

                return id;
            } else {
                throw new ApplicationException("El usuario no está autenticado");
            }
        }

        public async Task<int> CrearUsuario(UsuarioModel usuario) { 
            using var connection = new SqlConnection(configurationString);
            var usuarioID = await connection.QuerySingleAsync<int>(@"INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
                                                              VALUES (@Email, @EmailNormalizado, @PasswordHash);
                                                              SELECT SCOPE_IDENTITY();", usuario);

            await connection.ExecuteAsync("CrearDatosUsuarioNuevo", 
                                          new { usuarioID }, 
                                          commandType: System.Data.CommandType.StoredProcedure);

            return usuarioID;
        }

        public async Task<UsuarioModel> BuscarUsuarioByEmail(string emailNormalizado) {
            using var connection = new SqlConnection(configurationString);

            return await connection.QuerySingleOrDefaultAsync<UsuarioModel>(@"SELECT * FROM Usuarios
                                                                              WHERE EmailNormalizado = @EmailNormalizado",
                                                                              new { emailNormalizado });
        }
    }
}
