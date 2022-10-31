using ManejoPresupuesto.Models;

namespace ManejoPresupuesto.Services {
    public class ReportesRepository : IReportesRepository {
        private readonly ITransaccionesRepository transaccionesRepository;
        private readonly HttpContext httpContext;

        public ReportesRepository(ITransaccionesRepository transaccionesRepository,
                                  IHttpContextAccessor httpContextAccessor) {
            this.transaccionesRepository = transaccionesRepository;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        /* Genera valores de las fechas */
        private (DateTime fechaInicio, DateTime fechaFin) GenerarFecha(int mes, int anio) {
            /* Inicializa las fecha */
            DateTime fechaInicio;
            DateTime fechaFin;

            if (mes <= 0 || mes > 12 || anio <= 1900) {
                var hoy = DateTime.Today;
                fechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            } else {
                fechaInicio = new DateTime(anio, mes, 1);
            }

            fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            return (fechaInicio, fechaFin);
        }

        /* Genera el reporte de transacciones de forma detallada */
        private static ReporteTransacciones GenerarReporteTransaccionesDetalladas(DateTime fechaInicio, DateTime fechaFin, IEnumerable<TransaccionModel> transacciones) {
            var modelo = new ReporteTransacciones();

            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion)
                                                     .GroupBy(x => x.FechaTransaccion)
                                                     .Select(grupo => new ReporteTransacciones.TransaccionesPorFecha() {
                                                         FechaTransaccion = grupo.Key,
                                                         Transacciones = grupo.AsEnumerable()
                                                     });

            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = fechaInicio;
            modelo.FechaFin = fechaFin;
            return modelo;
        }

        /* Asigna valores al ViewBag */
        private void AsignarValoresViewBag(dynamic ViewBag, DateTime fechaInicio) {
            ViewBag.MesAnterior = fechaInicio.AddMonths(-1).Month;
            ViewBag.AnioAnterior = fechaInicio.AddMonths(-1).Year;
            ViewBag.MesPosterior = fechaInicio.AddMonths(1).Month;
            ViewBag.AnioPosterior = fechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }

        /* Obtiene el reporte de las transacciones para una cuenta */
        public async Task<ReporteTransacciones> ObtenerReporteTransaccionesByCuenta(int usuarioID, int cuentaID, int mes, int anio, dynamic ViewBag) {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFecha(mes, anio);

            var transaccionesPorCuenta = new TransaccionesPorCuenta() {
                CuentaID = cuentaID,
                UsuarioID = usuarioID,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await transaccionesRepository.ObtenerTransaccionByCuentaID(transaccionesPorCuenta);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);
            AsignarValoresViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        /* Obtiene el reporte de transacciones de forma detallada */
        public async Task<ReporteTransacciones> ObtenerReporteTransaccionesDetalladas(int usuarioID, int mes, int anio, dynamic ViewBag) {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFecha(mes, anio);

            var parametro = new TransaccionesPorUsuarioModel() {
                UsuarioID = usuarioID,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var transacciones = await transaccionesRepository.ObtenerTransaccionByUsuarioID(parametro);
            var modelo = GenerarReporteTransaccionesDetalladas(fechaInicio, fechaFin, transacciones);
            AsignarValoresViewBag(ViewBag, fechaInicio);

            return modelo;
        }

        /* Obtiene el reporte de transacciones por semana */
        public async Task<IEnumerable<ResultadoPorSemana>> ObtenerReporteSemanal(int usuarioID, int mes, int anio, dynamic ViewBag) {
            (DateTime fechaInicio, DateTime fechaFin) = GenerarFecha(mes, anio);

            var parametro = new TransaccionesPorUsuarioModel() {
                UsuarioID = usuarioID,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            AsignarValoresViewBag(ViewBag, fechaInicio);
            var modelo = await transaccionesRepository.ObtenerBySemana(parametro);

            return modelo;
        }

        /* Obtiene el reporte de transacciones mensualmente */
    }
}
