using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers {
    public class UsuariosController : Controller {
        private readonly UserManager<UsuarioModel> userManager;

        public UsuariosController(UserManager<UsuarioModel> userManager) {
            this.userManager = userManager;
        }

        public IActionResult Registro() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(RegistroModel modelo) {
            if (!ModelState.IsValid) { 
                return View(modelo);
            }

            var usuario = new UsuarioModel() { 
                Email = modelo.Email
            };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded) {
                return RedirectToAction("Index", "Transacciones");
            } else {
                foreach (var error in resultado.Errors) {
                    ModelState.AddModelError(String.Empty, error.Description);
                }

                return View(modelo);
            }            
        }
    }
}
