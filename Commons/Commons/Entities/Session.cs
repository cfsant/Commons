using Commons.Interfaces;
using System;

namespace Entities.Public
{
    public class Session : ISession
    {
        public string Token { get; set; }
        public Guid? Id { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? PublisherId { get; set; }
        public DateTime Insertion { get; set; }
        public DateTime Change { get; set; }
    }
}
