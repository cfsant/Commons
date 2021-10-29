using Commons.Interfaces;

namespace Commons.Entities
{
    public class BaseStateful : Base, IBase, IBaseStateful
    {
        public bool State { get; set; }
    }
}
