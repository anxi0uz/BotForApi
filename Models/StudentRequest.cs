using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bot.Models
{
    public class StudentRequest
    {
        public string fio { get; set; }

        public int idSpec { get; set; }

        public int birthday { get; set; }

        public string uchebnoeZav { get; set; }

        public string phoneNumber { get;set; }

        public string address { get; set; }  

        public int age {  get; set; }   
    }
}
