namespace ManejoPresupuesto.Models {
    public class ResultadoPorMes {
        public int Mes { get; set; }
        public DateTime FechaReferencia { get; set; }
        public decimal Monto { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Gasto { get; set; }
        public TipoOperacionModel TipoOperacionID { get; set; }
    }
}
