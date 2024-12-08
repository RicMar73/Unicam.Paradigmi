using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicam.Paradigmi.Models.DTOs
{
    public class UserRegistrationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // Usa un nome più intuitivo rispetto a PasswordHash
    }

}
