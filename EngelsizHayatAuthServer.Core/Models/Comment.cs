using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public long CreateTime { get; set; }
        public string Text { get; set; }
        public long Like { get; set; }
        public virtual Post Post{ get; set; }
        public virtual ICollection<NestedComment> NestedComments { get; set; }
        public virtual UserApp UserApp { get; set; }
    }
}
