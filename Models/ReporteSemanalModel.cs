namespace ManejoPresupuesto.Models {
    public class ReporteSemanalModel {
        public decimal Ingresos => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public decimal Gastos => TransaccionesPorSemana.Sum(x => x.Gastos);
        public decimal Total => Ingresos - Gastos;
        public DateTime FechaReferencia { get; set; }
        public IEnumerable<ResultadoPorSemana> TransaccionesPorSemana { get; set; }
    }
}
