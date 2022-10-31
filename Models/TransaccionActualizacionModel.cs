namespace ManejoPresupuesto.Models {
    public class TransaccionActualizacionModel : TransaccionCreacionModel {
        public int CuentaAnteriorID { get; set; }
        public decimal MontoAnterior { get; set; }
        public string urlRetorno { get; set; }
    }
}
