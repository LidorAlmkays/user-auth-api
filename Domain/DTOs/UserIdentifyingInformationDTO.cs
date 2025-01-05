using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class UserIdentifyingInformationDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public required string Email { get; set; }
    }
}