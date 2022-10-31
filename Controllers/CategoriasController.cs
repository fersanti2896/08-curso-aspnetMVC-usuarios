using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ManejoPresupuesto.Controllers {
    public class CategoriasController : Controller {
        private readonly ICategoriasRepository categoriasRepository;
        private readonly IUsuarioRepository usuarioRepository;

        public CategoriasController(ICategoriasRepository categoriasRepository,
                                    IUsuarioRepository usuarioRepository) {
            this.categoriasRepository = categoriasRepository;
            this.usuarioRepository = usuarioRepository;
        }

        /* Vista para el listado de Categorias */
        public async Task<IActionResult> Index() {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var categorias = await categoriasRepository.ObtenerCategorias(usuarioID);

            return View(categorias);   
        }

        /* Vista para crear categoria */
        [HttpGet]
        public IActionResult Crear() { 
            return View();
        }

        /* Crear Categoria */
        [HttpPost]
        public async Task<IActionResult> Crear(CategoriaModel categoria) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();

            if (!ModelState.IsValid) { 
                return View(categoria);
            }

            categoria.UsuarioID = usuarioID;
            await categoriasRepository.CrearCategoria(categoria);

            return RedirectToAction("Index");
        }

        /* Vista para actualizar una categoria */
        [HttpGet]
        public async Task<IActionResult> Editar(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var categoria = await categoriasRepository.ObtenerCategoriaById(id, usuarioID);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        /* Actualiza la categoria */
        [HttpPost]
        public async Task<IActionResult> Editar(CategoriaModel categoriaEditar) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var categoria = await categoriasRepository.ObtenerCategoriaById(categoriaEditar.Id, usuarioID);

            if (!ModelState.IsValid) {
                return View(categoriaEditar);
            }

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            categoriaEditar.UsuarioID = usuarioID;
            await categoriasRepository.ActualizarCategoria(categoriaEditar);

            return RedirectToAction("Index");
        }

        /* Vista para borrar categoria */
        [HttpGet]
        public async Task<IActionResult> Borrar(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var categoria = await categoriasRepository.ObtenerCategoriaById(id, usuarioID);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoria);
        }

        /* Borra la categoria */
        [HttpPost]
        public async Task<IActionResult> BorrarCategoria(int id) {
            var usuarioID = usuarioRepository.ObtenerUsuarioID();
            var categoria = await categoriasRepository.ObtenerCategoriaById(id, usuarioID);

            if (categoria is null) {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await categoriasRepository.BorrarCategoria(id);

            return RedirectToAction("Index");
        }
    }
}
