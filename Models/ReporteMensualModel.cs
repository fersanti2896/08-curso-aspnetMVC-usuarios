namespace ManejoPresupuesto.Models {
    public class ReporteMensualModel {
        public IEnumerable<ResultadoPorMes> TransaccionesMes { get; set; }
        public decimal Ingresos => TransaccionesMes.Sum(x => x.Ingreso);
        public decimal Gastos => TransaccionesMes.Sum(x => x.Gasto);
        public decimal Total => Ingresos - Gastos;
        public int Anio { get; set; }

    }
}
