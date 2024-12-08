using System.ComponentModel.DataAnnotations;

namespace Unicam.Paradigmi.Models.Models
{
    public class User
    {
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }

}
