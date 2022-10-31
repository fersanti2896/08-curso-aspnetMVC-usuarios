using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Models {
    public class CuentaCreacionModel : CuentaModel {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
