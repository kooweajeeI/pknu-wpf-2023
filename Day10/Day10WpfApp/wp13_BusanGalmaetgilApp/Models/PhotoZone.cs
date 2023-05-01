using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp13_BusanGalmaetgilApp.Models
{
    public class PhotoZone
    {
        //  "seq": "CF02020004",
        //    "cf_gbn": "포토존",
        //    "cf_pos": "남천동 메가마트 부근 큰 사거리",
        //    "cf_addr": "부산광역시 수영구 남천1동 566",
        //    "cf_sat": "양호",
        //    "name": "광안대교를 배경",-
        //    "course": "2",
        //    "gugan": "2",
        //    "lng": "129.111874791295",
        //    "lat": "35.1364331670244"

        public string Seq { get; set; }
        public string Cf_gbn { get; set; }
        public string Cf_pos { get; set; }
        public string Cf_Addr { get; set; }
        public string Cf_sat { get; set; }
        public string Name { get; set; }
        public int Course { get; set; }
        public int Gugan { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
    }
}
