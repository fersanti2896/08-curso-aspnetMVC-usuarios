using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public interface IReportesRepository {
        Task<IEnumerable<ResultadoPorSemana>> ObtenerReporteSemanal(int usuarioID, int mes, int anio, dynamic ViewBag);
        Task<ReporteTransacciones> ObtenerReporteTransaccionesByCuenta(int usuarioID, int cuentaID, int mes, int anio, dynamic ViewBag);
        Task<ReporteTransacciones> ObtenerReporteTransaccionesDetalladas(int usuarioID, int mes, int anio, dynamic ViewBag);
    }
}
