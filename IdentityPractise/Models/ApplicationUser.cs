using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace IdentityPractise.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;


        public byte[]? ProfilePicture { get; set; } = null!;

        public string? GetProfilePictureUrl()
        {
            if (ProfilePicture == null)
            {
                return null;
            }
            string imageBase64Data =
                Convert.ToBase64String(ProfilePicture);
            return $"data:image/*;base64,{imageBase64Data}";
        } 
    }
}
