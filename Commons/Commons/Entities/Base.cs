using Commons.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace Commons.Entities
{
    public class Base : IBase
    {
        [Key]
        public Guid? Id { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? PublisherId { get; set; }
        public DateTime Insertion { get; set; }
        public DateTime Change { get; set; }
    }
}
