using ManejoPresupuesto.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers {
    public class UsuariosController : Controller {
        private readonly UserManager<UsuarioModel> userManager;
        private readonly SignInManager<UsuarioModel> signInManager;

        public UsuariosController(UserManager<UsuarioModel> userManager,
                                  SignInManager<UsuarioModel> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Registro() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroModel modelo) {
            if (!ModelState.IsValid) { 
                return View(modelo);
            }

            var usuario = new UsuarioModel() { 
                Email = modelo.Email
            };

            var resultado = await userManager.CreateAsync(usuario, password: modelo.Password);

            if (resultado.Succeeded) {
                await signInManager.SignInAsync(usuario, isPersistent: true);

                return RedirectToAction("Index", "Transacciones");
            } else {
                foreach (var error in resultado.Errors) {
                    ModelState.AddModelError(String.Empty, error.Description);
                }

                return View(modelo);
            }            
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model) {
            if (!ModelState.IsValid) { 
                return View(model);   
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.Recuerdame, lockoutOnFailure: false);

            if (result.Succeeded) {
                return RedirectToAction("Index", "Transacciones");
            } else {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos");

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            return RedirectToAction("Index", "Transacciones");
        }
    }
}
