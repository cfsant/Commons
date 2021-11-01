using Commons.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commons.Entities
{
    public class ChangeRecordTracker : BaseNamed, IChangeRecordTracker
    {
        [ForeignKey("ChangeRecord")]
        public Guid ChangeRecordId { get; set; }
        public Guid RecordIdentifier { get; set; }

        public virtual ChangeRecord ChangeRecord { get; set; }
    }
}
