using Commons.Interfaces;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Commons.Entities
{
    public class ChangeRecord : BaseNamedStateful, IChangeRecord
    {
        public Guid RecordIdentifier { get; set; }
        public string SerializedPrevious { get; set; }
        public string SerializedCurrent { get; set; }

        [NotMapped]
        public object Previous { 
            get {
                return JsonConvert.DeserializeObject(this.SerializedPrevious, Type.GetType(this.Name));
            }
        }

        [NotMapped]
        public object Current
        {
            get
            {
                return JsonConvert.DeserializeObject(this.SerializedCurrent, Type.GetType(this.Name));
            }
        }
    }
}
