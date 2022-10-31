using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Services {
    public class CategoriasRepository : ICategoriasRepository {
        private readonly string connectionString;

        public CategoriasRepository(IConfiguration configuration) {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /* Obtiene el listado de categorias */
        public async Task<IEnumerable<CategoriaModel>> ObtenerCategorias(int usuarioID) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<CategoriaModel>(@"SELECT * FROM Categorias
                                                                 WHERE UsuarioID = @UsuarioID", new { usuarioID });
        }

        /* Crea una categoria */
        public async Task CrearCategoria(CategoriaModel categoria) { 
            using var connection = new SqlConnection(connectionString);
            
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO 
                                                              Categorias (Nombre, TipoOperacionID, UsuarioID)
                                                              VALUES (@Nombre, @TipoOperacionID, @UsuarioID);

                                                              SELECT SCOPE_IDENTITY();", categoria);

            categoria.Id = id;  
        }

        /* Obtiene la información de una categoria por su id */
        public async Task<CategoriaModel> ObtenerCategoriaById(int id, int usuarioID) { 
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryFirstOrDefaultAsync<CategoriaModel>(@"SELECT * FROM Categorias
                                                                               WHERE Id = @Id AND UsuarioID = @UsuarioID",
                                                                             new { id, usuarioID });
        }

        /* Actualiza una categoria */
        public async Task ActualizarCategoria(CategoriaModel categoria) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"UPDATE Categorias
                                            SET Nombre = @Nombre, TipoOperacionID = @TipoOperacionID, UsuarioID = @UsuarioID
                                            WHERE Id = @Id",
                                           categoria);
        }

        /* Borrando una categoria */
        public async Task BorrarCategoria(int id) {
            using var connection = new SqlConnection(connectionString);

            await connection.ExecuteAsync(@"DELETE Categorias WHERE Id = @Id", new { id });
        }

        /* Obtiene las categorias en base al tipo de operacion */
        public async Task<IEnumerable<CategoriaModel>> ObtenerCategoriasByTipoOperacion(int usuarioID, TipoOperacionModel tipoOperacionID) {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<CategoriaModel>(@"SELECT * FROM Categorias
                                                                 WHERE UsuarioID = @UsuarioID AND TipoOperacionID = @TipoOperacionID",
                                                                 new { usuarioID, tipoOperacionID });
        }
    }
}
