using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bot.Models
{
    public record Studdent(
    int? id,
    string? fio,
    string? phoneNumber,
    string? address,
    int? age,
    string? specName);
    //public class Student
    //{
    //    public int Id { get; set; }

    //    public string Fio { get; set; } 

    //    public int IdSpec { get; set; }

    //    public int Birthday { get; set; }

    //    public string Uchebnoezav { get; set; }

    //    public string PhoneNumber { get; set; }

    //    public string Adress { get; set; } 

    //    public int Age { get; set; }

    //}
}
