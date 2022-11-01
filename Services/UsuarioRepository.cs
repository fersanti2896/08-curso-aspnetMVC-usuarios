using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services {
    public class UsuarioRepository : IUsuarioRepository {
        private readonly string configurationString;

        public UsuarioRepository(IConfiguration configuration) {
            configurationString = configuration.GetConnectionString("DefaultConnection");
        }

        public int ObtenerUsuarioID() {
            return 1;
        }

        public async Task<int> CrearUsuario(UsuarioModel usuario) { 
            using var connection = new SqlConnection(configurationString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO Usuarios (Email, EmailNormalizado, PasswordHash)
                                                              VALUES (@Email, @EmailNormalizado, @PasswordHash)", usuario);

            return id;
        }

        public async Task<UsuarioModel> BuscarUsuarioByEmail(string emailNormalizado) {
            using var connection = new SqlConnection(configurationString);

            return await connection.QuerySingleOrDefaultAsync<UsuarioModel>(@"SELECT * FROM Usuarios
                                                                              WHERE EmailNormalizado = @EmailNormalizado",
                                                                              new { emailNormalizado });
        }
    }
}
