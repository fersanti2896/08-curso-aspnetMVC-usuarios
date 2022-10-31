using System.ComponentModel.DataAnnotations;

namespace ManejoPresupuesto.Models {
    public class TransaccionModel {
        public int Id { get; set; }

        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Fecha Transacción")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        public decimal Monto { get; set; }

        [StringLength(maximumLength: 1000, ErrorMessage = "La nota no debe pasar de {1} caracteres")]
        public string Nota { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Cuenta")]
        [Range(0, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una cuenta")]
        public int CuentaID { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Categoría")]
        [Range(0, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una categoría")]
        public int CategoriaID { get; set; }

        [Display(Name = "Tipo de Operación")]
        public TipoOperacionModel TipoOperacionId { get; set; } = TipoOperacionModel.Ingreso;

        public string Cuenta { get; set; }
        public string Categoria { get; set; }
    }
}
