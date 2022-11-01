using AutoMapper;
using ClosedXML.Excel;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ManejoPresupuesto.Controllers {

    public class TransaccionesController : Controller {
        private readonly IUsuarioRepository usuarioRepository;
        private readonly ITransaccionesRepository transaccionesRepository;
        private readonly ICuentasRepository cuentasRepository;
        private readonly ICategoriasRepository categoriasRepository;
        private readonly IReportesRepository reportesRepository;
        private readonly IMapper mapper;

        public TransaccionesController(IUsuarioRepository usuarioRepository,
                                       ITransaccionesRepository transaccionesRepository, 
                                       ICuentasRepository cuentasRepository, 
                                       ICategoriasRepository categoriasRepository,
                                       IReportesRepository reportesRepository,
                                       IMapper mapper) {
            this.usuarioRepository = usuarioRepository;
            this.transaccionesRepository = transaccionesRepository;
            this.cuentasRepository = cuentasRepository;
            this.categoriasRepository = categoriasRepository;
            this.reportesRepository = reportesRepository;
            this.mapper = mapper;
        }

        /* Listado de transacciones por usuario */
        public async Task<IActionResult> Index(int mes, int anio) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var modelo = await reportesRepository.ObtenerReporteTransaccionesDetalladas(usuarioID, mes, anio, ViewBag);

            return View(modelo);
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioID) {
            var cuentas = await cuentasRepository.ListadoCuentas(usuarioID);

            return cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ListadoCategoriasByTipoOperacion(int usuarioID, TipoOperacionModel tipoOperacion) { 
            var categorias = await categoriasRepository.ObtenerCategoriasByTipoOperacion(usuarioID, tipoOperacion);

            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        /* Obtiene las categorias en base al tipo de operacion */
        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacionModel tipoOperacion) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var categorias = await ListadoCategoriasByTipoOperacion(usuarioID, tipoOperacion);

            return Ok(categorias);
        }

        /* Vista para crear una Transacción */
        public async Task<IActionResult> Crear() {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var modelo = new TransaccionCreacionModel();

            modelo.Cuentas = await ObtenerCuentas(usuarioID);
            modelo.Categorias = await ListadoCategoriasByTipoOperacion(usuarioID, modelo.TipoOperacionId);

            return View(modelo);
        }

        /* Guarda la transacción creada en BD */
        [HttpPost]
        public async Task<IActionResult> Crear(TransaccionCreacionModel modelo) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();

            if (!ModelState.IsValid) {
                modelo.Cuentas = await ObtenerCuentas(usuarioID);
                modelo.Categorias = await ListadoCategoriasByTipoOperacion(usuarioID, modelo.TipoOperacionId);

                return View(modelo);
            }

            var cuenta = await cuentasRepository.ObtenerCuentaById(modelo.CuentaID, usuarioID);
            
            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await categoriasRepository.ObtenerCategoriaById(modelo.CategoriaID, usuarioID);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioID = usuarioID;

            if (modelo.TipoOperacionId == TipoOperacionModel.Gasto) {
                modelo.Monto *= -1;
            }

            await transaccionesRepository.Crear(modelo);

            return RedirectToAction("Index");
        }

        /* Crea la vista para actualizar una transacción */
        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var transaccion = await transaccionesRepository.ObtenerTransaccionById(id, usuarioID);

            if (transaccion is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizacionModel>(transaccion);
            modelo.MontoAnterior = modelo.Monto;

            if (modelo.TipoOperacionId == TipoOperacionModel.Gasto) {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorID = transaccion.CuentaID;
            modelo.Categorias = await ListadoCategoriasByTipoOperacion(usuarioID, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioID);
            modelo.urlRetorno = urlRetorno;

            return View(modelo);
        }

        /* Actualiza la información de la transacción */
        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizacionModel modelo) { 
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            
            if (!ModelState.IsValid) {
                modelo.Cuentas = await ObtenerCuentas(usuarioID);
                modelo.Categorias = await ListadoCategoriasByTipoOperacion(usuarioID, modelo.TipoOperacionId);

                return View(modelo);
            }

            var cuenta = await cuentasRepository.ObtenerCuentaById(modelo.CuentaID, usuarioID);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await categoriasRepository.ObtenerCategoriaById(modelo.CategoriaID, usuarioID);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var transaccion = mapper.Map<TransaccionModel>(modelo);

            if (modelo.TipoOperacionId == TipoOperacionModel.Gasto) {
                transaccion.Monto *= -1;
            }

            await transaccionesRepository.ActualizarTransaccion(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorID);

            if (string.IsNullOrEmpty(modelo.urlRetorno)) {
                return RedirectToAction("Index");
            } else {
                return LocalRedirect(modelo.urlRetorno);
            }            
        }

        /* Elimina una transacción */
        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var transaccion = await transaccionesRepository.ObtenerTransaccionById(id, usuarioID);

            if (transaccion is null) { 
                return RedirectToAction("NoEncontrado", "Home");
            }

            await transaccionesRepository.BorrarTransaccion(id);

            if (string.IsNullOrEmpty(urlRetorno)) {
                return RedirectToAction("Index");
            } else {
                return LocalRedirect(urlRetorno);
            }
        }

        /* Reporte Semanal */
        public async Task<IActionResult> Semanal(int mes, int anio) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            IEnumerable<ResultadoPorSemana> transaccionesSemana = await reportesRepository.ObtenerReporteSemanal(usuarioID, mes, anio, ViewBag);

            /* Algoritmo */
            var agrupado = transaccionesSemana.GroupBy(x => x.Semana)
                                              .Select(x => new ResultadoPorSemana() {
                                                  Semana = x.Key,
                                                  Ingresos = x.Where(x => x.TipoOperacionID == TipoOperacionModel.Ingreso)
                                                              .Select(x => x.Monto)
                                                              .FirstOrDefault(),
                                                  Gastos = x.Where(x => x.TipoOperacionID == TipoOperacionModel.Gasto)
                                                              .Select(x => x.Monto)
                                                              .FirstOrDefault()
                                              })
                                              .ToList();

            if (anio == 0 || mes == 0) {
                var hoy = DateTime.Today;
                anio = hoy.Year;
                mes = hoy.Month;
            }

            var fechaReferencia = new DateTime(anio, mes, 1);
            var diasMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);
            var diasSegmentado = diasMes.Chunk(7).ToList();

            for (int i = 0; i < diasSegmentado.Count(); i++) {
                var semana = i + 1;
                var fechaInicio = new DateTime(anio, mes, diasSegmentado[i].First());
                var fechaFin = new DateTime(anio, mes, diasSegmentado[i].Last());
                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana);

                if (grupoSemana is null) {
                    agrupado.Add(new ResultadoPorSemana() { 
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin
                    });
                } else {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }
            }

            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();

            var modelo = new ReporteSemanalModel();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaReferencia = fechaReferencia;

            return View(modelo);
        }

        /* Reporte Mensual */
        public async Task<IActionResult> Mensual(int anio) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();

            if (anio == 0) {
                anio = DateTime.Today.Year;    
            }

            var transaccionesMes = await transaccionesRepository.ObtenerByMes(usuarioID, anio);
            var transaccionesAgrupadas = transaccionesMes.GroupBy(x => x.Mes)
                                                         .Select(x => new ResultadoPorMes() {
                                                            Mes = x.Key,
                                                            Ingreso = x.Where(x => x.TipoOperacionID == TipoOperacionModel.Ingreso)
                                                                       .Select(x => x.Monto)
                                                                       .FirstOrDefault(),
                                                            Gasto = x.Where(x => x.TipoOperacionID == TipoOperacionModel.Gasto)
                                                                     .Select(x => x.Monto)
                                                                     .FirstOrDefault()
                                                         })
                                                         .ToList();

            for (int mes = 1; mes <= 12; mes++) {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(anio, mes, 1);

                if (transaccion is null) { 
                    transaccionesAgrupadas.Add(new ResultadoPorMes() {  
                        Mes = mes, 
                        FechaReferencia = fechaReferencia
                    });
                } else {
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }

            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending(x => x.Mes).ToList();
            var modelo = new ReporteMensualModel();
            modelo.Anio = anio;
            modelo.TransaccionesMes = transaccionesAgrupadas;

            return View(modelo);
        }

        /* Genera el Excel */
        private FileResult GenerarExcel(string nombreArchivo, IEnumerable<TransaccionModel> transacciones) {
            DataTable dataTable = new DataTable("Transacciones");
            dataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("Fecha"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Nota"),
                new DataColumn("Monto"),
                new DataColumn("Ingreso/Gasto"),
            });

            foreach (var transaccion in transacciones) {
                dataTable.Rows.Add(transaccion.FechaTransaccion,
                                   transaccion.Cuenta,
                                   transaccion.Categoria,
                                   transaccion.Nota,
                                   transaccion.Monto,
                                   transaccion.TipoOperacionId
                                   );
            }

            using (XLWorkbook wb = new XLWorkbook()) {
                wb.Worksheets.Add(dataTable);
                
                using (MemoryStream stream = new MemoryStream()) {
                    wb.SaveAs(stream);

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreArchivo);
                } 
            }
        }

        /* Exportar Excel por Mes */
        [HttpGet]
        public async Task<FileResult> ExportarExcelMes(int mes, int anio) {
            var fechaInicio = new DateTime(anio, mes, 1);
            var fecharFin = fechaInicio.AddMonths(1).AddDays(-1);
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var transacciones = await transaccionesRepository.ObtenerTransaccionByUsuarioID(new TransaccionesPorUsuarioModel {
                UsuarioID = usuarioID,
                FechaInicio = fechaInicio,
                FechaFin = fecharFin
            });
            var nombreArchivo = $"Manejo Presupuesto - { fechaInicio.ToString("MMMM yyyy") }.xlsx";

            /* Genera el excel */
            return GenerarExcel(nombreArchivo, transacciones);
        }

        /* Exportar Excel por Año */
        [HttpGet]
        public async Task<FileResult> ExportarExcelAnio(int anio) {
            var fechaInicio = new DateTime(anio, 1, 1);
            var fechaFin = fechaInicio.AddYears(1).AddDays(-1);
            var usuarioID = usuarioRepository.ObtenerUsuarioID();

            var transacciones = await transaccionesRepository.ObtenerTransaccionByUsuarioID(new TransaccionesPorUsuarioModel {
                UsuarioID = usuarioID,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var nombreArchivo = $"Manejo Presupuesto - { fechaInicio.ToString("yyyy") }.xlsx";

            /* Genera el excel */
            return GenerarExcel(nombreArchivo, transacciones);
        }

        /* Exportar Excel General */
        [HttpGet]
        public async Task<FileResult> ExportarExcelTodo() {
            var fechaInicio = DateTime.Today.AddYears(-100);
            var fechaFin = DateTime.Today.AddYears(1000);
            var usuarioID = usuarioRepository.ObtenerUsuarioID();

            var transacciones = await transaccionesRepository.ObtenerTransaccionByUsuarioID(new TransaccionesPorUsuarioModel {
                UsuarioID = usuarioID,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var nombreArchivo = $"Manejo Presupuesto - { DateTime.Today.ToString("dd-MM-yyyy") }.xlsx";

            /* Genera el excel */
            return GenerarExcel(nombreArchivo, transacciones);
        }

        /* Reporte Excel */
        public IActionResult Excel() {
            return View();
        }

        public async Task<JsonResult> ObtenerTransaccionesCalendario(DateTime start, DateTime end) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();

            var transacciones = await transaccionesRepository.ObtenerTransaccionByUsuarioID(new TransaccionesPorUsuarioModel {
                UsuarioID = usuarioID,
                FechaInicio = start,
                FechaFin = end
            });

            var eventos = transacciones.Select(transaccion => new EventoCalendario() { 
                Title = transaccion.Monto.ToString("N"),
                Start = transaccion.FechaTransaccion.ToString("yyyy-MM-dd"),
                End = transaccion.FechaTransaccion.ToString("yyyy-MM-dd"),
                Color = (transaccion.TipoOperacionId == TipoOperacionModel.Gasto) ? "Red" : null
            });

            return Json(eventos);
        }

        public async Task<JsonResult> ObtenerTransaccionFecha(DateTime fecha) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();

            var transacciones = await transaccionesRepository.ObtenerTransaccionByUsuarioID(new TransaccionesPorUsuarioModel {
                UsuarioID = usuarioID,
                FechaInicio = fecha,
                FechaFin = fecha
            });

            return Json(transacciones);
        }

        /* Reporte Calendario */
        public IActionResult Calendario() {
            return View();
        }
    }
}
