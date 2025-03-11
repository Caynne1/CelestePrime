using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CelestePrime.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; } // Unique constraint will be set in Fluent API

        [Required]
        public required string Username { get; set; }

        [NotMapped] // This field is NOT saved in the database
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        [Required]
        public required string Role { get; set; } // New property for user roles
    }
}
