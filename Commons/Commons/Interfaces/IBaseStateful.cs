namespace Commons.Interfaces
{
    public interface IBaseStateful : IBase
    {
        bool State { get; set; }
    }
}
