using Commons.Interfaces;

namespace Commons.Entities
{
    public class BaseNamedStateful : Base, IBaseNamedStateful
    {
        public string Name { get; set; }
        public bool State { get; set; }
    }
}
