using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class PublicMessageDTO
    {
        public int MessageId { get; set; }    
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTime PubDate { get; set; }
        public int Flagged { get; set; }
    }
}
