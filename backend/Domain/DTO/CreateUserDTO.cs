using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTO
{
    public class CreateUserDTO
    {
        public string username { get; set; }
        public string email { get; set; }
        public string pwd { get; set; }
    }
}
