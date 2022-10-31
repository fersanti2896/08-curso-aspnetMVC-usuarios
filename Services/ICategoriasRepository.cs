using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public interface ICategoriasRepository {
        Task ActualizarCategoria(CategoriaModel categoria);
        Task BorrarCategoria(int id);
        Task CrearCategoria(CategoriaModel categoria);
        Task<CategoriaModel> ObtenerCategoriaById(int id, int usuarioID);
        Task<IEnumerable<CategoriaModel>> ObtenerCategorias(int usuarioID);
        Task<IEnumerable<CategoriaModel>> ObtenerCategoriasByTipoOperacion(int usuarioID, TipoOperacionModel tipoOperacion);
    }
}
