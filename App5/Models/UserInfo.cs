using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App5.Models
{
    public class UserInfo
    {
        public int id {  get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string BirthDate { get; set; }
        public string Role {  get; set; }
        // 身份证号
        public string ID_number { get; set; }
        public string Phone { get; set; }
        public string idAndName => $"{id} - {Name}";
    }
}
