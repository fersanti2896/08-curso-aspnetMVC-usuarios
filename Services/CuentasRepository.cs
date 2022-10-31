using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services {
    public class CuentasRepository : ICuentasRepository {
        private readonly string connectionString;

        public CuentasRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /* Crea una cuenta */
        public async Task Crear(CuentaModel cuenta) { 
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO 
                                                         Cuentas (Nombre, TipoCuentaId, Balance, Descripcion)
                                                         VALUES (@Nombre, @TipoCuentaId, @Balance, @Descripcion);

                                                         SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }

        /* Trae todo el listado de cuenta por el id del usuario */
        public async Task<IEnumerable<CuentaModel>> ListadoCuentas(int usuarioID) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<CuentaModel>(@"SELECT C.Id, C.Nombre, Balance, TC.Nombre AS TipoCuenta 
                                                              FROM Cuentas C
                                                              INNER JOIN TiposCuentas TC
                                                              ON TC.Id = C.TipoCuentaId
                                                              WHERE TC.UsuarioID = @UsuarioID
                                                              ORDER BY TC.Orden", new { usuarioID });
        }

        /* Obtener cuenta por id */
        public async Task<CuentaModel> ObtenerCuentaById(int id, int usuarioID) { 
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<CuentaModel>(@"SELECT C.Id, C.Nombre, Balance, C.Descripcion, C.TipoCuentaId
                                                                            FROM Cuentas C
                                                                            INNER JOIN TiposCuentas TC
                                                                            ON TC.Id = C.TipoCuentaId
                                                                            WHERE TC.UsuarioID = @UsuarioID AND C.Id = @Id",
                                                                            new { id, usuarioID });
        }

        /* Actualiza la cuenta */
        public async Task ActualizaCuenta(CuentaCreacionModel cuenta) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE Cuentas
                                            SET Nombre = @Nombre, Balance = @Balance, Descripcion = @Descripcion, TipoCuentaId = @TipoCuentaId
                                            WHERE Id = @Id", cuenta);    
        }

        /* Borra cuenta */
        public async Task Borrar(int id) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"DELETE Cuentas WHERE Id = @Id", new { id });
        }
    }
}
