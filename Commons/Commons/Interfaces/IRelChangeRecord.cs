using Commons.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Interfaces
{
    public interface IRelChangeRecord
    {
        [ForeignKey("ChangeRecord")]
        Guid ChangeRecordId { get; set; }

        ChangeRecord ChangeRecord { get; set; }
    }
}
