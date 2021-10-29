using Commons.Interfaces;

namespace Commons.Entities
{
    public class BaseNamed : Base, IBase, IBaseNamed
    {
        public string Name { get; set; }
    }
}
