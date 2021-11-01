using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IChangeRecordTracker : IRelChangeRecord, IBaseNamed
    {
        Guid RecordIdentifier { get; set; }
    }
}
