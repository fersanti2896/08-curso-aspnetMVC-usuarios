using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public interface ICuentasRepository {
        Task ActualizaCuenta(CuentaCreacionModel cuenta);
        Task Borrar(int id);
        Task Crear(CuentaModel cuenta);
        Task<IEnumerable<CuentaModel>> ListadoCuentas(int usuarioID);
        Task<CuentaModel> ObtenerCuentaById(int id, int usuarioID);
    }
}
