using AutoMapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers {
    public class CuentasController : Controller {
        private readonly ITiposCuentasRepository tiposCuentasRepository;
        private readonly IUsuarioRepository usuarioRepository;
        private readonly ICuentasRepository cuentasRepository;
        private readonly IMapper mapper;
        private readonly ITransaccionesRepository transaccionesRepository;
        private readonly IReportesRepository reportesRepository;

        public CuentasController(ITiposCuentasRepository tiposCuentasRepository, 
                                 IUsuarioRepository usuarioRepository, 
                                 ICuentasRepository cuentasRepository,
                                 IMapper mapper,
                                 ITransaccionesRepository transaccionesRepository, 
                                 IReportesRepository reportesRepository) {
            this.tiposCuentasRepository = tiposCuentasRepository;
            this.usuarioRepository = usuarioRepository;
            this.cuentasRepository = cuentasRepository;
            this.mapper = mapper;
            this.transaccionesRepository = transaccionesRepository;
            this.reportesRepository = reportesRepository;
        }

        /* Listado de Cuentas */
        public async Task<IActionResult> Index(){
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var cuentasTipoCuenta = await cuentasRepository.ListadoCuentas(usuarioID);

            var modelo = cuentasTipoCuenta.GroupBy(x => x.TipoCuenta)
                                          .Select(grupo => new IndiceCuentaModel {
                                              TipoCuenta = grupo.Key,
                                              Cuentas = grupo.AsEnumerable()
                                          })
                                          .ToList();

            return View(modelo);
        }

        /* Obtiene el detalle de la cuenta */
        [HttpGet]
        public async Task<IActionResult> Detalle(int id, int mes, int anio) { 
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var cuenta = await cuentasRepository.ObtenerCuentaById(id, usuarioID);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            ViewBag.Cuenta = cuenta.Nombre;
            var modelo = await reportesRepository.ObtenerReporteTransaccionesByCuenta(usuarioID, id, mes, anio, ViewBag);

            return View(modelo);
        }

        /* Muestra el formulario para crear una cuenta */
        [HttpGet]
        public async Task<IActionResult> Crear() {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var modelo = new CuentaCreacionModel();

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioID);

            return View(modelo);
        }

        /* Crea una cuenta */
        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionModel cuenta) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var tipoCuenta = await tiposCuentasRepository.ObtenerTipoCuentaById(cuenta.TipoCuentaId, usuarioID);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid) {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioID);

                return View(cuenta);
            }

            await cuentasRepository.Crear(cuenta);

            return RedirectToAction("Index");
        }

        /* Obtiene el listado de tipos cuenta para select en el formulario */
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioID) {
            var tiposCuentas = await tiposCuentasRepository.ObtenerListadoByUsuarioID(usuarioID);

            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        /* Obtiene la informacion para su posterior edición */
        [HttpGet]
        public async Task<IActionResult> Editar(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var cuenta = await cuentasRepository.ObtenerCuentaById(id, usuarioID);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            /* Mapeo manual */
            /* var modelo = new CuentaCreacionModel() { 
                Id = cuenta.Id,
                Nombre = cuenta.Nombre,
                TipoCuentaId = cuenta.TipoCuentaId,
                Descripcion = cuenta.Descripcion,  
                Balance = cuenta.Balance
            }; */

            /* Mapeo con AutoMapper */
            var modelo = mapper.Map<CuentaCreacionModel>(cuenta);

            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioID);

            return View(modelo);
        }

        /* Actualiza la cuenta */
        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionModel cuentaEditar) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var cuenta = await cuentasRepository.ObtenerCuentaById(cuentaEditar.Id, usuarioID);   

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipoCuenta = await tiposCuentasRepository.ObtenerTipoCuentaById(cuentaEditar.TipoCuentaId, usuarioID);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            /* Actualiza la cuenta */
            await cuentasRepository.ActualizaCuenta(cuentaEditar);

            return RedirectToAction("Index");
        }

        /* Muestra vista con la información de la cuenta a borrar */
        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var cuenta = await cuentasRepository.ObtenerCuentaById(id, usuarioID);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(cuenta);
        }

        /* Elimina la cuenta */
        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var cuenta = await cuentasRepository.ObtenerCuentaById(id, usuarioID);

            if (cuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await cuentasRepository.Borrar(id);

            return RedirectToAction("Index");
        }
    }
}
