using System.ComponentModel.DataAnnotations;

namespace MiProp.Models
{
    public class Edificio
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(150)]
        public string Direccion { get; set; }

        public string AdminId { get; set; }
        public Usuario Admin { get; set; }

        public ICollection<Departamento> Departamentos { get; set; } = new List<Departamento>();
    }
}
