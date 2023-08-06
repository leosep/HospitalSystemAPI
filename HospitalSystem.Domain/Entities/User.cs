using System.ComponentModel.DataAnnotations;

namespace HospitalSystem.Domain.Entities
{
    public class User
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
