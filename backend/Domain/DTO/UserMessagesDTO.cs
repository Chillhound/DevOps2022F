using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class UserMessagesDTO
    {
        public bool IsFollowing { get; set; }
        public List<PublicMessageDTO> messages { get; set; }
    }
}
