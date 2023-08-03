using System.ComponentModel.DataAnnotations;

namespace TriviaGameApp.Models
{
    public class User
    {
        [Key] // The Key attribute denotes the primary key
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // other properties here
    }
}
