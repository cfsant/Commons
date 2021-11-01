using Commons.Interfaces;

namespace Commons.Entities
{
    public class Profile : BaseNamedStateful, IProfile
    {
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
