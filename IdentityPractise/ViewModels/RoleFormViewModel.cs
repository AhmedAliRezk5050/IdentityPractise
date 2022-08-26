using System.ComponentModel.DataAnnotations;

namespace IdentityPractise.ViewModels;

public class RoleFormViewModel
{
    [Required, MaxLength(250)]
    public string Name { get; set; } = null!;
}