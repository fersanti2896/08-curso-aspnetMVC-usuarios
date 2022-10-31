using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public interface ITiposCuentasRepository {
        Task ActualizarTipoCuenta(TipoCuentaModel tipoCuenta);
        Task BorrartipoCuenta(int id);
        Task Crear(TipoCuentaModel tipoCuenta);
        Task<bool> ExisteTipoCuenta(string nombre, int usuarioID);
        Task<IEnumerable<TipoCuentaModel>> ObtenerListadoByUsuarioID(int usuarioID);
        Task<TipoCuentaModel> ObtenerTipoCuentaById(int id, int usuarioID);
        Task OrdenarTiposCuentas(IEnumerable<TipoCuentaModel> tipoCuentasOrdenados);
    }
}
