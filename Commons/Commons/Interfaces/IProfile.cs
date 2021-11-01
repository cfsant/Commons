namespace Commons.Interfaces
{
    public interface IProfile : IBaseNamedStateful
    {
        string Password { get; set; }
        string[] Roles { get; set; }
    }
}
