using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services {
    public class TransaccionesRepository : ITransaccionesRepository {
        private readonly string connectionString;

        public TransaccionesRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /* Creando Transaccion */
        public async Task Crear(TransaccionModel transaccion) {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>("Transacciones_NuevoRegistro", 
                                                            new { 
                                                                transaccion.UsuarioID, 
                                                                transaccion.FechaTransaccion, 
                                                                transaccion.Monto,
                                                                transaccion.CategoriaID, 
                                                                transaccion.CuentaID, 
                                                                transaccion.Nota 
                                                            },
                                                            commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        /* Obtiene una transacción por su id y el id del usuario */
        public async Task<TransaccionModel> ObtenerTransaccionById(int id, int usuarioID) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<TransaccionModel>(@"SELECT Transacciones.*, Cat.TipoOperacionID
                                                                                 FROM Transacciones
                                                                                 INNER JOIN Categorias Cat
                                                                                 ON Cat.Id = Transacciones.CategoriaID
                                                                                 WHERE Transacciones.Id = @Id AND Transacciones.UsuarioID = @UsuarioID",
                                                                                 new { id, usuarioID });
        }

        /* Actualizando Transaccion */
        public async Task ActualizarTransaccion(TransaccionModel transaccion, decimal montoAnterior, int cuentaAnteriorID) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transacciones_Actualizar", new { 
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaID,
                transaccion.CuentaID,
                transaccion.Nota,
                montoAnterior, 
                cuentaAnteriorID
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        /* Borrando una Transacción */
        public async Task BorrarTransaccion(int id) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync("Transacciones_Borrar", 
                                           new { id }, 
                                           commandType: System.Data.CommandType.StoredProcedure);
        }

        /* Busca una transacción por cuenta */
        public async Task<IEnumerable<TransaccionModel>> ObtenerTransaccionByCuentaID(TransaccionesPorCuenta transaccionesPorCuenta) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<TransaccionModel>(@"SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre AS Categoria, Q.Nombre AS Cuenta, C.TipoOperacionID 
                                                                   FROM Transacciones T
                                                                   INNER JOIN Categorias C
                                                                   ON C.Id = T.CategoriaID
                                                                   INNER JOIN Cuentas Q
                                                                   ON Q.Id = T.CuentaID
                                                                   WHERE T.CuentaID = @CuentaID AND 
                                                                         T.UsuarioID = @UsuarioID AND 
                                                                         FechaTransaccion BETWEEN @FechaInicio AND 
                                                                         @FechaFin ", transaccionesPorCuenta);
        }

        /* Busca una transacción por usuario */
        public async Task<IEnumerable<TransaccionModel>> ObtenerTransaccionByUsuarioID(TransaccionesPorUsuarioModel modelo) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<TransaccionModel>(@"SELECT T.Id, T.Monto, T.FechaTransaccion, C.Nombre AS Categoria, Q.Nombre AS Cuenta, C.TipoOperacionID, Nota
                                                                   FROM Transacciones T
                                                                   INNER JOIN Categorias C
                                                                   ON C.Id = T.CategoriaID
                                                                   INNER JOIN Cuentas Q
                                                                   ON Q.Id = T.CuentaID
                                                                   WHERE T.UsuarioID = @UsuarioID AND 
                                                                         FechaTransaccion BETWEEN @FechaInicio AND 
                                                                         @FechaFin
                                                                   ORDER BY T.FechaTransaccion DESC", modelo);
        }

        /*  -------------- REPORTES ------------ */
        /* Reporte por Semana */
        public async Task<IEnumerable<ResultadoPorSemana>> ObtenerBySemana(TransaccionesPorUsuarioModel modelo) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoPorSemana>(@"SELECT DATEDIFF(d, @FechaInicio, FechaTransaccion) / 7 + 1 AS Semana,
                                                                     SUM(Monto) AS Monto, Cat.TipoOperacionID
                                                                     FROM Transacciones T
                                                                     INNER JOIN Categorias Cat
                                                                     ON Cat.Id = T.CategoriaID
                                                                     WHERE T.UsuarioID = @UsuarioID AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                                                                     GROUP BY DATEDIFF(d, @FechaInicio, FechaTransaccion) / 7, Cat.TipoOperacionID",
                                                                    modelo);
        }

        /* Reporte por Mes */
        public async Task<IEnumerable<ResultadoPorMes>> ObtenerByMes(int usuarioID, int anio) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<ResultadoPorMes>(@"SELECT MONTH(T.FechaTransaccion) AS Mes, SUM(T.Monto) AS Monto, Cat.TipoOperacionID
                                                                  FROM Transacciones T
                                                                  INNER JOIN Categorias Cat
                                                                  ON Cat.Id = T.CategoriaID
                                                                  WHERE T.UsuarioID = @UsuarioID AND YEAR(T.FechaTransaccion) = @Anio
                                                                  GROUP BY MONTH(T.FechaTransaccion), Cat.TipoOperacionID",
                                                                  new { usuarioID, anio });
        }
    }
}
