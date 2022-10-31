using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers {
    public class TiposCuentasController : Controller {
        private readonly ITiposCuentasRepository tiposCuentasRepository;
        private readonly IUsuarioRepository usuarioRepository;

        public TiposCuentasController(ITiposCuentasRepository tiposCuentasRepository, 
                                      IUsuarioRepository usuarioRepository) {
            this.tiposCuentasRepository = tiposCuentasRepository;
            this.usuarioRepository = usuarioRepository;
        }

        public IActionResult Crear() {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuentaModel tipoCuenta) {
            if (!ModelState.IsValid) {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioID = usuarioRepository.ObtenerUsuarioID();

            var existeTipoCuenta = await tiposCuentasRepository.ExisteTipoCuenta(tipoCuenta.Nombre, tipoCuenta.UsuarioID);

            if (existeTipoCuenta) {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre { tipoCuenta.Nombre } ya existe!");

                return View(tipoCuenta);
            }

            await tiposCuentasRepository.Crear(tipoCuenta);

            return RedirectToAction("Index");
        }

        /* Verifica la existencia de un tipo de cuenta desde JS */
        [HttpGet]
        public async Task<IActionResult> VerificaExistenciaTipoCuenta(string nombre) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var existeTipoCuenta = await tiposCuentasRepository.ExisteTipoCuenta(nombre, usuarioID);

            if (existeTipoCuenta) {
                return Json($"¡El nombre { nombre } ya existe!");
            }

            return Json(true);
        }

        /* Listado de Tipos Cuentas por Usuario ID */
        public async Task<IActionResult> Index() {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var tiposCuentas = await tiposCuentasRepository.ObtenerListadoByUsuarioID(usuarioID);

            return View(tiposCuentas);
        }

        /* Recupera un tipo de cuenta para actualizar */
        [HttpGet]
        public async Task<ActionResult> Editar(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var tipoCuenta = await tiposCuentasRepository.ObtenerTipoCuentaById(id, usuarioID);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        /* Actualiza un tipo de cuenta */
        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuentaModel tipoCuenta) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var tipoCuentaExiste = await tiposCuentasRepository.ObtenerTipoCuentaById(tipoCuenta.Id, usuarioID);

            if (tipoCuentaExiste is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await tiposCuentasRepository.ActualizarTipoCuenta(tipoCuenta);

            return RedirectToAction("Index");
        }

        /* Recupera un tipo de cuenta para borrar */
        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var tipoCuenta = await tiposCuentasRepository.ObtenerTipoCuentaById(id, usuarioID);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(tipoCuenta);
        }

        /* Aplica el borrado de tipo cuenta */
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var tipoCuenta = await tiposCuentasRepository.ObtenerTipoCuentaById(id, usuarioID);

            if (tipoCuenta is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await tiposCuentasRepository.BorrartipoCuenta(id);

            return RedirectToAction("Index");
        }

        /* Ordena el listado en base a como se mapean en el front */
        [HttpPost]
        public async Task<IActionResult> OrdenarTipoCuenta([FromBody] int[] ids) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var tiposCuentas = await tiposCuentasRepository.ObtenerListadoByUsuarioID(usuarioID);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            /* Verifica que los ids que se reciben sean del usuario */
            var idsTiposCuentasNoUser = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoUser.Count > 0) {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((val, index) => 
                                                    new TipoCuentaModel() { 
                                                        Id = val,
                                                        Orden = index + 1
                                                    }).AsEnumerable();

            await tiposCuentasRepository.OrdenarTiposCuentas(tiposCuentasOrdenados);

            return Ok();
        }
    }
}
