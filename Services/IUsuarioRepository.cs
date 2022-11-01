using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public interface IUsuarioRepository {
        Task<UsuarioModel> BuscarUsuarioByEmail(string emailNormalizado);
        Task<int> CrearUsuario(UsuarioModel usuario);
        int ObtenerUsuarioID();
    }
}
