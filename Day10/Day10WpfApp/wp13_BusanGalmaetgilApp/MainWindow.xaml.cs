using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using wp13_BusanGalmaetgilApp.Logics;
using wp13_BusanGalmaetgilApp.Models;

namespace wp13_BusanGalmaetgilApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // openAPI 조회
        // 화장실 리스트
        string apiKey = "dANBYPs2ySIfvBBvMxJm390IPNYxlsfHrJbutZr5XAWEbv6My303AIP35%2FwYy7G%2FLhMX5rBiCyRzZLvtpF3KkQ%3D%3D";
        private async void BtnRestroomList_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = $@"https://apis.data.go.kr/6260000/fbusangmgadvantigeinfo/getgmgrestroominfo?serviceKey={apiKey}&numOfRows=10&pageNo=1&resultType=json";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            // var status = Convert.ToInt32(jsonResult["status"]);
            var resultCode = Convert.ToString(jsonResult["getgmgrestroominfo"]["header"]["resultCode"]);

            try
            {
                if (resultCode == "00")  // 정상이면 데이터 받아서 처리
                {
                    var data = jsonResult["getgmgrestroominfo"]["body"]["items"]["item"];
                    var json_array = data as JArray;

                    var conFacilities = new List<Facilities>();
                    foreach (var galmaetgil in json_array)
                    {
                        conFacilities.Add(new Facilities
                        {
                            Gbn = Convert.ToString(galmaetgil["gbn"]),
                            Name = Convert.ToString(galmaetgil["name"]),
                            Addr_r = Convert.ToString(galmaetgil["addr_r"]),
                            Addr_j = Convert.ToString(galmaetgil["addr_j"]),
                            Both_sexes = Convert.ToString(galmaetgil["both_sexes"]),
                            Manager = Convert.ToString(galmaetgil["manager"]),
                            Tel_no = Convert.ToString(galmaetgil["tel_no"]),
                            Open_time = Convert.ToString(galmaetgil["open_time"]),
                            Setup_date = Convert.ToString(galmaetgil["setup_date"]),
                            Alarm_bell = Convert.ToString(galmaetgil["alarm_bell"]),
                            Course = Convert.ToInt32(galmaetgil["course"]),
                            Gugan = Convert.ToInt32(galmaetgil["gugan"]),
                            Lng = Convert.ToDouble(galmaetgil["lng"]),
                            Lat = Convert.ToDouble(galmaetgil["lat"])
                        });
                    }
                    this.DataContext = conFacilities;  // 데이터 넘어오는지 확인
                    StsResult.Content = $"OpenAPI {conFacilities.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");
            }
        }

        // 포토존 리스트
        private async void BtnPhotozoneList_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = $@"https://apis.data.go.kr/6260000/fbusangmgadvantigeinfo/getgmgphotoinfo?serviceKey={apiKey}&numOfRows=10&pageNo=1&resultType=json";
            string result = string.Empty;

            // WebRequest, WebResponse 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                req = WebRequest.Create(openApiUri);
                res = await req.GetResponseAsync();
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(result);
            // var status = Convert.ToInt32(jsonResult["status"]);
            var resultCode = Convert.ToString(jsonResult["getgmgphotoinfo"]["header"]["resultCode"]);

            try
            {
                if (resultCode == "00")  // 정상이면 데이터 받아서 처리
                {
                    var data = jsonResult["getgmgphotoinfo"]["body"]["items"]["item"];
                    var json_array = data as JArray;

                    var photozoneList = new List<PhotoZone>();
                    foreach (var galmaetgil in json_array)
                    {
                        photozoneList.Add(new PhotoZone
                        {
                            Seq = Convert.ToString(galmaetgil["seq"]),
                            Cf_gbn = Convert.ToString(galmaetgil["cf_gbn"]),
                            Cf_pos = Convert.ToString(galmaetgil["cf_pos"]),
                            Cf_Addr = Convert.ToString(galmaetgil["cf_addr"]),
                            Cf_sat = Convert.ToString(galmaetgil["cf_sat"]),
                            Name = Convert.ToString(galmaetgil["name"]),
                            Course = Convert.ToInt32(galmaetgil["course"]),
                            Gugan = Convert.ToInt32(galmaetgil["gugan"]),
                            Lng = Convert.ToDouble(galmaetgil["lng"]),
                            Lat = Convert.ToDouble(galmaetgil["lat"])
                        });
                    }
                    this.DataContext = photozoneList;  // 데이터 넘어오는지 확인
                    StsResult.Content = $"OpenAPI {photozoneList.Count}건 조회완료";

                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");
            }
        }



        // 검색한 결과 DB(MySQL)에 저장
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 저장하세요.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    var query = @"INSERT INTO facilities
                                            (Gbn,
                                            Name,
                                            Addr_r,
                                            Addr_j,
                                            Both_sexes,
                                            Manager,
                                            Tel_no,
                                            Open_time,
                                            Setup_date,
                                            Alarm_bell,
                                            Course,
                                            Gugan,
                                            Lng,
                                            Lat)
                                            VALUES                             
                                            (@Gbn,
                                            @Name,
                                            @Addr_r,
                                            @Addr_j,
                                            @Both_sexes,
                                            @Manager,
                                            @Tel_no,
                                            @Open_time,
                                            @Setup_date,
                                            @Alarm_bell,
                                            @Course,
                                            @Gugan,
                                            @Lng,
                                            @Lat)";
                    var insRes = 0;
                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is Facilities)
                        {
                            var item = temp as Facilities;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Gbn", item.Gbn);
                            cmd.Parameters.AddWithValue("@Name", item.Name);
                            cmd.Parameters.AddWithValue("@Addr_r", item.Addr_r);
                            cmd.Parameters.AddWithValue("@Addr_j", item.Addr_j);
                            cmd.Parameters.AddWithValue("@Both_sexes", item.Both_sexes);
                            cmd.Parameters.AddWithValue("@Manager", item.Manager);
                            cmd.Parameters.AddWithValue("@Tel_no", item.Tel_no);
                            cmd.Parameters.AddWithValue("@Open_time", item.Open_time);
                            cmd.Parameters.AddWithValue("@Setup_date", item.Setup_date);
                            cmd.Parameters.AddWithValue("@Alarm_bell", item.Alarm_bell);
                            cmd.Parameters.AddWithValue("@Course", item.Course);
                            cmd.Parameters.AddWithValue("@Gugan", item.Gugan);
                            cmd.Parameters.AddWithValue("@Lng", item.Lng);
                            cmd.Parameters.AddWithValue("@Lat", item.Lat);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }

                    await Commons.ShowMessageAsync("저장", $"DB 저장 성공!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";

                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장 오류 {ex.Message}");
            }
        }

        private async void BtnSavePzList_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 저장하세요.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    var query = @"INSERT INTO photozone
                                            (seq,
                                            cf_gbn,
                                            cf_pos,
                                            cf_addr,
                                            cf_sat,
                                            name,
                                            course,
                                            gugan,
                                            lng,
                                            lat)
                                            VALUES
                                            (@seq,
                                            @cf_gbn,
                                            @cf_pos,
                                            @cf_addr,
                                            @cf_sat,
                                            @name,
                                            @course,
                                            @gugan,
                                            @lng,
                                            @lat)";
                    var insRes = 0;
                    foreach (var temp in GrdResult.Items)
                    {
                        if (temp is PhotoZone)
                        {
                            var item = temp as PhotoZone;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Seq", item.Seq);
                            cmd.Parameters.AddWithValue("@Cf_gbn", item.Cf_gbn);
                            cmd.Parameters.AddWithValue("@Cf_pos", item.Cf_pos);
                            cmd.Parameters.AddWithValue("@Cf_Addr", item.Cf_Addr);
                            cmd.Parameters.AddWithValue("@Cf_sat", item.Cf_sat);
                            cmd.Parameters.AddWithValue("@Name", item.Name);
                            cmd.Parameters.AddWithValue("@Course", item.Course);
                            cmd.Parameters.AddWithValue("@Gugan", item.Gugan);
                            cmd.Parameters.AddWithValue("@Lng", item.Lng);
                            cmd.Parameters.AddWithValue("@Lat", item.Lat);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }

                    await Commons.ShowMessageAsync("저장", $"DB 저장 성공!!");
                    StsResult.Content = $"DB저장 {insRes}건 성공";

                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장 오류 {ex.Message}");
            }
        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = GrdResult.SelectedItem;
            var selRestroom = GrdResult.SelectedItem as Facilities;
            var selPhotozone = GrdResult.SelectedItem as PhotoZone;

            if (selectedItem is Facilities)
            {
                var mapWindow = new MapWindow(selRestroom.Lat, selRestroom.Lng);
                mapWindow.Owner = this;
                mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                mapWindow.ShowDialog();
            }
            else if (selectedItem is PhotoZone)
            {
                var mapWindow = new MapWindow(selPhotozone.Lat, selPhotozone.Lng);
                mapWindow.Owner = this;
                mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                mapWindow.ShowDialog();
            }
        }

        private void CboFacilities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboFacilities.SelectedValue != null)
            {
                // MessageBox.Show(CboReqDate.SelectedValue.ToString());
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT Gbn,
                                        Name,
                                        Addr_r,
                                        Addr_j,
                                        Both_sexes,
                                        Manager,
                                        Tel_no,
                                        Open_time,
                                        Setup_date,
                                        Alarm_bell,
                                        Course,
                                        Gugan,
                                        Lng,
                                        Lat
                                    FROM facilities
                                   Where SUBSTRING(addr_j, 7, 7) = @addr_j";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Addr_j", CboFacilities.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "Facilities");
                    List<Facilities> fcs = new List<Facilities>();
                    foreach (DataRow row in ds.Tables["Facilities"].Rows)
                    {
                        fcs.Add(new Facilities
                        {
                            Gbn = Convert.ToString(row["gbn"]),
                            Name = Convert.ToString(row["name"]),
                            Addr_r = Convert.ToString(row["addr_r"]),
                            Addr_j = Convert.ToString(row["addr_j"]),
                            Both_sexes = Convert.ToString(row["both_sexes"]),
                            Manager = Convert.ToString(row["manager"]),
                            Tel_no = Convert.ToString(row["tel_no"]),
                            Open_time = Convert.ToString(row["open_time"]),
                            Setup_date = Convert.ToString(row["setup_date"]),
                            Alarm_bell = Convert.ToString(row["alarm_bell"]),
                            Course = Convert.ToInt32(row["course"]),
                            Gugan = Convert.ToInt32(row["gugan"]),
                            Lng = Convert.ToDouble(row["lng"]),
                            Lat = Convert.ToDouble(row["lat"])
                        });
                    }

                    this.DataContext = fcs;
                    //StsResult.Content = $"OpenAPI {Facilities.Count}건 조회완료";
                }
            }
            else
            {
                this.DataContext = null;
                StsResult.Content = $"DB 조회클리어";
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT SUBSTRING(addr_j, 7, 7) as a FROM facilities
                                WHERE addr_j like '%동%' group by 1;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["a"]));
                }
                CboFacilities.ItemsSource = saveDateList;
            }

            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT SUBSTRING(cf_addr, 7, INSTR(SUBSTR(cf_addr, 7), ' ') + 3) AS result
                            FROM photozone WHERE cf_addr like '%동%' group by 1;";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["result"]));
                }
                CboPhotoZone.ItemsSource = saveDateList;
            }
        }

        private void CboPhotoZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CboPhotoZone.SelectedValue != null)
            {
                // MessageBox.Show(CboReqDate.SelectedValue.ToString());
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    conn.Open();
                    var query = @"SELECT seq,
                                        cf_gbn,
                                        cf_pos,
                                        cf_addr,
                                        cf_sat,
                                        name,
                                        course,
                                        gugan,
                                        lng,
                                        lat
                                    FROM photozone
                                   WHERE SUBSTRING(cf_addr, 7, INSTR(SUBSTR(cf_addr, 7), ' ') + 3) = @cf_addr";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@cf_addr", CboPhotoZone.SelectedValue.ToString());
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, "PhotoZone");
                    List<PhotoZone> aaa = new List<PhotoZone>();
                    foreach (DataRow galmaetgil in ds.Tables["PhotoZone"].Rows)
                    {
                        aaa.Add(new PhotoZone
                        {
                            Seq = Convert.ToString(galmaetgil["seq"]),
                            Cf_gbn = Convert.ToString(galmaetgil["cf_gbn"]),
                            Cf_pos = Convert.ToString(galmaetgil["cf_pos"]),
                            Cf_Addr = Convert.ToString(galmaetgil["cf_addr"]),
                            Cf_sat = Convert.ToString(galmaetgil["cf_sat"]),
                            Name = Convert.ToString(galmaetgil["name"]),
                            Course = Convert.ToInt32(galmaetgil["course"]),
                            Gugan = Convert.ToInt32(galmaetgil["gugan"]),
                            Lng = Convert.ToDouble(galmaetgil["lng"]),
                            Lat = Convert.ToDouble(galmaetgil["lat"])
                        });
                    }

                    this.DataContext = aaa;
                    //StsResult.Content = $"OpenAPI {Facilities.Count}건 조회완료";
                }
            }
            else
            {
                this.DataContext = null;
                StsResult.Content = $"DB 조회클리어";
            }
        }

        //private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // 콤보박스에 들어갈 날짜를 DB에서 불러와
        //    // 저장한 뒤에도 콤보박스 재조회해야 날짜가 전부 출력
        //    using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
        //    {
        //        conn.Open();
        //        var query = @"SELECT SUBSTRING(addr_j, 7, 7) FROM facilities
        //                        WHERE addr_j like '%동%' group by 1;";
        //        MySqlCommand cmd = new MySqlCommand(query, conn);
        //        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        //        DataSet ds = new DataSet();
        //        adapter.Fill(ds);
        //        List<string> saveDateList = new List<string>();
        //        foreach (DataRow row in ds.Tables[0].Rows)
        //        {
        //            saveDateList.Add(Convert.ToString(row["Save_Date"]));
        //        }
        //        CboFacilities.ItemsSource = saveDateList;
        //    }
        //}

        //// 그리드 특정Row 더블클릭 새창에 센서위치 출력
        //private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    var selItem = GrdResult.SelectedItem as DustSensor;

        //    var mapWindow = new MapWindow(selItem.Coordy, selItem.Coordx);  // 부모창 위치값을 자식창으로 전달
        //    mapWindow.Owner = this;     // MainWindow 부모
        //    mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;    // 부모창 중간에 출력
        //    mapWindow.ShowDialog();
        //}
    }
}
