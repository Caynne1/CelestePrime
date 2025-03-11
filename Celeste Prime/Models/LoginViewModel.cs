using System.ComponentModel.DataAnnotations;

namespace CelestePrime.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty; // Ensures it's initialized

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Ensures it's initialized
    }
}
