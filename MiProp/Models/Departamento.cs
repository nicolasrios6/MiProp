using System.ComponentModel.DataAnnotations;

namespace MiProp.Models
{
    public class Departamento
    {
        public int Id { get; set; }
        [Required]
        public string Numero { get; set; }
        public int? Piso { get; set; }

        public int EdificioId { get; set; }
        public Edificio Edificio { get; set; }

        public string? InquilinoId { get; set; }
        public Usuario? Inquilino { get; set; }
    }
}
