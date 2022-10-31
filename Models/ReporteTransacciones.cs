namespace ManejoPresupuesto.Models {
    public class ReporteTransacciones {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(x => x.BalanceDepositos);
        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public decimal Total => BalanceDepositos - BalanceRetiros;

        public class TransaccionesPorFecha {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<TransaccionModel> Transacciones { get; set; }
            public decimal BalanceDepositos => Transacciones.Where(x => x.TipoOperacionId == TipoOperacionModel.Ingreso)
                                                            .Sum(x => x.Monto);

            public decimal BalanceRetiros => Transacciones.Where(x => x.TipoOperacionId == TipoOperacionModel.Gasto)
                                                            .Sum(x => x.Monto);
        }
    }
}
