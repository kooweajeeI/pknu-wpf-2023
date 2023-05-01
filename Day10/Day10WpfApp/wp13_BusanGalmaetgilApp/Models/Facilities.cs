using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp13_BusanGalmaetgilApp.Models
{
    public class Facilities
    {
        /*
            "gbn": "공중화장실",
            "name": "광안리해수욕장 생활문화센터(지하) 공중화장실",
            "addr_r": "부산광역시 수영구 광안해변로 219(광안동)",
            "addr_j": "부산광역시 수영구 광안동 192-20",
            "both_sexes": "N",
            "manager": "부산광역시 수영구청 자원순환과",
            "tel_no": "051-610-4445",
            "open_time": "24시",
            "setup_date": "2000-01-01",
            "alarm_bell": "여자",
            "course": "2",
            "gugan": "2",
            "lng": "129.118533",
            "lat": "35.15377436"
        */

        public string Gbn { get; set; }
        public string Name { get; set; }
        public string Addr_r { get; set; }
        public string Addr_j { get; set; }
        public string Both_sexes { get; set; }
        public string Manager { get; set; }
        public string Tel_no { get; set; } 
        public string Open_time { get; set; }
        public string Setup_date { get; set; }
        public string Alarm_bell { get; set; }
        public int Course { get; set; }
        public int Gugan { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
    }
}
