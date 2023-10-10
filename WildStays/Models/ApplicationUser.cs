using Microsoft.AspNetCore.Identity;

namespace WildStays.DAL
{
    //Applicationuser extends Identityuser, the default user model when using ApsNetCore.Identity.
    public class ApplicationUser : IdentityUser
    {
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
