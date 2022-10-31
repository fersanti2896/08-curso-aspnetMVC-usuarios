using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers {
    public class UsuariosController : Controller {

        public IActionResult Registro() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroModel modelo) {
            if (!ModelState.IsValid) { 
                return View(modelo);
            }

            return RedirectToAction("Index", "Transacciones");
        }
    }
}
