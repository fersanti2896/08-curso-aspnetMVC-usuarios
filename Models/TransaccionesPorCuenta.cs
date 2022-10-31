namespace ManejoPresupuesto.Models {
    public class TransaccionesPorCuenta {
        public int UsuarioID { get; set; }
        public int CuentaID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
    }
}
