using System.ComponentModel.DataAnnotations.Schema;

namespace MiProp.Models
{
    public class Pago
    {
        public int Id { get; set; }

        public string InquilinoId { get; set; }
        public Usuario Inquilino { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }

        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;
    }

    public enum EstadoPago
    {
        Pendiente,
        Pagado,
        Vencido
    }
}
