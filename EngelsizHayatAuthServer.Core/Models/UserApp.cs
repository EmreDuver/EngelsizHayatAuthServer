using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngelsizHayatAuthServer.Core.Models
{
    public class UserApp : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoPath { get; set; }
        public DateTime BirthDay { get; set; }
        public int GenderId { get; set; }
        public string Biography { get; set; }
        public virtual Location Location { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
        public virtual ICollection<Comment> Comments{ get; set; }
        public virtual ICollection<NestedComment> NestedComments { get; set; }
    }
}
