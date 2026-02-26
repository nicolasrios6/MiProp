using System.ComponentModel.DataAnnotations;

namespace MiProp.Models.ViewModels
{
    public class AdminEditVM
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [Required]
        [MaxLength(100)]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool Activo { get; set; }

    }
}
