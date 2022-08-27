namespace IdentityPractise.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public IEnumerable<string> Roles { get; set; } = null!;
}