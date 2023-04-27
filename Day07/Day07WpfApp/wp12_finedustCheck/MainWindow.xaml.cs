using MahApps.Metro.Controls;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
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
using wp12_finedustCheck.Logics;
using wp12_finedustCheck.Models;

namespace wp12_finedustCheck
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

        // 김해시 OpenAPI 조회
        private async void BtnReqRealtime_Click(object sender, RoutedEventArgs e)
        {
            string openApiUri = "https://smartcity.gimhae.go.kr/smart_tour/dashboard/api/publicData/dustSensor";
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
            var status = Convert.ToInt32(jsonResult["status"]);

            try
            {
                if (status == 200)   // 정상이면 데이터 받아서 처리
                {
                    var data = jsonResult["data"];
                    var json_array = data as JArray;

                    var dustSensors = new List<DustSensor>();
                    foreach (var sensor in json_array)
                    {
                        dustSensors.Add(new DustSensor
                        {
                            Id = 0,
                            Dev_id = Convert.ToString(sensor["dev_id"]),
                            Name = Convert.ToString(sensor["name"]),
                            Loc = Convert.ToString(sensor["loc"]),
                            Coordx = Convert.ToDouble(sensor["coordx"]),
                            Coordy = Convert.ToDouble(sensor["coordy"]),
                            Ison = Convert.ToBoolean(sensor["ison"]),
                            Pm10_after = Convert.ToInt32(sensor["pm10_after"]),
                            Pm25_after = Convert.ToInt32(sensor["pm25_after"]),
                            State = Convert.ToInt32(sensor["state"]),
                            Timestamp = Convert.ToDateTime(sensor["timestamp"]),
                            Company_id = Convert.ToString(sensor["company_id"]),
                            Company_name = Convert.ToString(sensor["company_name"])
                        }); ;
                    }
                    this.DataContext = dustSensors;
                    StsResult.Content = $"OpenAPI {dustSensors.Count}건 조회완료";
                }

            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리오류 {ex.Message}");
            }


        }

        // 검색한 결과 DB(MySQL)에 저장
        private async void BtnSaveData_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "조회하고 저장하세요");
                return;
            }
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO dustsensor
                                            (Dev_id,
                                            Name,
                                            Loc,
                                            Coordx,
                                            Coordy,
                                            Ison,
                                            Pm10_after,
                                            Pm25_after,
                                            State,
                                            Timestamp,
                                            Company_id,
                                            Company_name)
                                            VALUES
                                            (@Dev_id,
                                            @Name,
                                            @Loc,
                                            @Coordx,
                                            @Coordy,
                                            @Ison,
                                            @Pm10_after,
                                            @Pm25_after,
                                            @State,
                                            @Timestamp,
                                            @Company_id,
                                            @Company_name);";

                    var insRes = 0;
                    foreach(var temp in GrdResult.Items)
                    {
                        if (temp is DustSensor)
                        {
                            var item = temp as DustSensor;

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@Dev_id", item.Dev_id);
                            cmd.Parameters.AddWithValue("@Name", item.Name);
                            cmd.Parameters.AddWithValue("@Loc", item.Loc);
                            cmd.Parameters.AddWithValue("@Coordx", item.Coordx);
                            cmd.Parameters.AddWithValue("@Coordy", item.Coordy);
                            cmd.Parameters.AddWithValue("@Ison", item.Ison);
                            cmd.Parameters.AddWithValue("@Pm10_after", item.Pm10_after);
                            cmd.Parameters.AddWithValue("@Pm25_after", item.Pm25_after);
                            cmd.Parameters.AddWithValue("@State", item.State);
                            cmd.Parameters.AddWithValue("@Timestamp", item.Timestamp);
                            cmd.Parameters.AddWithValue("@Company_id", item.Company_id);
                            cmd.Parameters.AddWithValue("@Company_name", item.Company_name);

                            insRes += cmd.ExecuteNonQuery();
                        }
                    }

                    await Commons.ShowMessageAsync("저장", "DB저장성공");
                    StsResult.Content = $"DB저장 {insRes}건 성공";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB 저장오류 {ex.Message}");
            }
        }

        // DB(MySQL)에서 조회 리스트 뿌리기
        private void CboReqDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 콤보박스에 들어갈 날짜를 DB에서 불러와서
            using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
            {
                conn.Open();
                var query = @"SELECT DATE_FORMAT(Timestamp, '%Y-%m-%d') AS Save_Date
                            FROM dustsensor GROUP BY 1 ORDER BY 1";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                List<string> saveDateList = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    saveDateList.Add(Convert.ToString(row["Save_Date"]));
                }

                CboReqDate.ItemsSource = saveDateList;
            }
        }
    }
}
