using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp13_BusanGalmaetgilApp.Models
{
    public class CourseInfo
    {
        public int Seq { get; set; }
        public string Course_nm { get; set; }
        public string Gugan_nm { get; set; }
        public string Gm_range { get; set; }
        public string Gm_degree { get; set; }
        public string Start_pls { get; set; }
        public string Start_addr { get; set; }
        public string Middle_pls { get; set; }
        public string Middle_adr { get; set; }
        public string End_pls { get; set; }
        public string End_addr { get; set; }
        public string Gm_course { get; set; }
        public string Gm_text { get; set; }
    }
}
