namespace Ghafar_Tajhiz_Admin.ViewModels.User
{
    public class UserListViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}