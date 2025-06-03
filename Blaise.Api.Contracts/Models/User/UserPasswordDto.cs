namespace Blaise.Api.Contracts.Models.User
{
    public class UserPasswordDto
    {
        public string Password { get; set; }

        public string CurrentlyLoggedInUser { get; set; }
    }
}
