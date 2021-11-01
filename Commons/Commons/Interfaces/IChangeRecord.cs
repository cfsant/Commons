using Newtonsoft.Json.Linq;
using System;

namespace Commons.Interfaces
{
    public interface IChangeRecord : IBaseNamedStateful
    {
        Guid RecordIdentifier { get; set; }
        string SerializedPrevious { get; set; }
        string SerializedCurrent { get; set; }

        object Previous { get; }
        object Current { get; }
    }
}
