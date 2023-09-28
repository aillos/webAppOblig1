using Microsoft.AspNetCore.Identity;

namespace WildStays.DAL
{
    public class ApplicationUser : IdentityUser
    {
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
