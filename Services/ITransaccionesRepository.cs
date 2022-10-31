using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public interface ITransaccionesRepository {
        Task ActualizarTransaccion(TransaccionModel transaccion, decimal montoAnterior, int cuentaAnterior);
        Task BorrarTransaccion(int id);
        Task Crear(TransaccionModel transaccion);
        Task<IEnumerable<ResultadoPorMes>> ObtenerByMes(int usuarioID, int anio);
        Task<IEnumerable<ResultadoPorSemana>> ObtenerBySemana(TransaccionesPorUsuarioModel modelo);
        Task<IEnumerable<TransaccionModel>> ObtenerTransaccionByCuentaID(TransaccionesPorCuenta transaccionesPorCuenta);
        Task<TransaccionModel> ObtenerTransaccionById(int id, int usuarioID);
        Task<IEnumerable<TransaccionModel>> ObtenerTransaccionByUsuarioID(TransaccionesPorUsuarioModel modelo);
    }
}
