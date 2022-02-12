using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Follower
    {
        public int WhoId { get; set; }
        public int WhomId { get; set; }

        public User Who { get; set; }
        public User Whom { get; set; }
    }
}
