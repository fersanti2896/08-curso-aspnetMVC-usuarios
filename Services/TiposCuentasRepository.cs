using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services {
    public class TiposCuentasRepository : ITiposCuentasRepository {
        /* Inserción de un tipo cuenta en la BD */
        private readonly string conecctionString;
        public TiposCuentasRepository(IConfiguration configuration) {
            conecctionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuentaModel tipoCuenta) { 
            using var connection = new SqlConnection(conecctionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar",
                                                            new { 
                                                                UsuarioID = tipoCuenta.UsuarioID,
                                                                Nombre = tipoCuenta.Nombre
                                                            }, 
                                                            commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.Id = id;
        }

        /* Verifica si existe una cuenta en la BD */
        public async Task<bool> ExisteTipoCuenta(string nombre, int usuarioID) {
            using var connection = new SqlConnection(conecctionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>($@"SELECT 1
                                                                           FROM TiposCuentas
                                                                           WHERE Nombre = @Nombre AND UsuarioID = @UsuarioID;", 
                                                                        new { nombre, usuarioID });

            return existe == 1;
        }

        /* Listado de Tipos Cuentas */
        public async Task<IEnumerable<TipoCuentaModel>> ObtenerListadoByUsuarioID(int usuarioID) {
            using var connection = new SqlConnection(conecctionString);

            return await connection.QueryAsync<TipoCuentaModel>(@"SELECT Id, Nombre, Orden 
                                                                  FROM TiposCuentas
                                                                  WHERE UsuarioID = @UsuarioID        
                                                                  ORDER BY Orden", new { usuarioID });
        }

        /* Obtiene la información del tipo de cuenta por su id */
        public async Task<TipoCuentaModel> ObtenerTipoCuentaById(int id, int usuarioID) {
            using var connection = new SqlConnection(conecctionString);

            return await connection.QueryFirstOrDefaultAsync<TipoCuentaModel>(@"SELECT Id, Nombre, Orden
                                                                                FROM TiposCuentas
                                                                                WHERE Id = @Id AND UsuarioID = @UsuarioID",
                                                                                new { id, usuarioID });
        }

        /* Actualizando un tipo de cuenta */
        public async Task ActualizarTipoCuenta(TipoCuentaModel tipoCuenta) {
            using var connection = new SqlConnection(conecctionString);

            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre = @Nombre
                                            WHERE Id = @Id", tipoCuenta);
        }

        /* Borrando un tipo de cuenta */
        public async Task BorrartipoCuenta(int id) {
            using var connection = new SqlConnection(conecctionString);

            await connection.ExecuteAsync(@"DELETE TiposCuentas
                                            WHERE Id = @Id", new { id });
        }

        /* Ordenar los Tipos Cuentas */
        public async Task OrdenarTiposCuentas(IEnumerable<TipoCuentaModel> tipoCuentasOrdenados) {
            using var connection = new SqlConnection(conecctionString);

            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Orden = @Orden
                                            WHERE Id = @Id", tipoCuentasOrdenados);
        }
    }
}
