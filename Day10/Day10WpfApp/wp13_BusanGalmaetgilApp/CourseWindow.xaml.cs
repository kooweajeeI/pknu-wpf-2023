using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Newtonsoft.Json.Linq;
using wp13_BusanGalmaetgilApp.Logics;
using wp13_BusanGalmaetgilApp.Models;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Ocsp;
using System.Xml;
using MySql.Data.MySqlClient;

namespace wp13_BusanGalmaetgilApp
{
    /// <summary>
    /// CourseWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CourseWindow : MetroWindow
    {
        bool isFavorite = false;
        static string apiKey = "dANBYPs2ySIfvBBvMxJm390IPNYxlsfHrJbutZr5XAWEbv6My303AIP35%2FwYy7G%2FLhMX5rBiCyRzZLvtpF3KkQ%3D%3D";
        // string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
        string openApiUri = $@"https://apis.data.go.kr/6260000/fbusangmgcourseinfo/getgmgcourseinfo?serviceKey={apiKey}&numOfRows=10&pageNo=1&resultType=json";
        string result = string.Empty;   // 결과값

        // api 실행 할 객체
        WebRequest req = null;
        WebResponse res = null;
        StreamReader reader = null;


        public CourseWindow()
        {
            InitializeComponent();

        }

        //private async void BtnNaverMovie_Click(object sender, RoutedEventArgs e)
        //{
        //    await Commons.ShowMessageAsync("네이버영화", "네이버영화 사이트로 갑니다.");
        //}

        // 전체 조회
        private async void BtnAll_Click(object sender, RoutedEventArgs e)
        {
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
            var resultCode = Convert.ToString(jsonResult["getgmgcourseinfo"]["header"]["resultCode"]);

            try
            {
                if (resultCode == "00")  // 정상이면 데이터 받아서 처리
                {
                    var data = jsonResult["getgmgcourseinfo"]["body"]["items"]["item"];
                    var json_array = data as JArray;

                    var GalMaetgilInfo = new List<CourseInfo>();
                    foreach (var courses in json_array)
                    {
                        GalMaetgilInfo.Add(new CourseInfo
                        {
                            Seq = Convert.ToInt32(courses["seq"]),
                            Course_nm = Convert.ToString(courses["course_nm"]),
                            Gugan_nm = Convert.ToString(courses["gugan_nm"]),
                            Gm_range = Convert.ToString(courses["gm_range"]),
                            Gm_degree = Convert.ToString(courses["gm_degree"]),
                            Start_pls = Convert.ToString(courses["start_pls"]),
                            Start_addr = Convert.ToString(courses["start_pls"]),
                            Middle_pls = Convert.ToString(courses["middle_pls"]),
                            Middle_adr = Convert.ToString(courses["middle_adr"]),
                            End_pls = Convert.ToString(courses["end_pls"]),
                            End_addr = Convert.ToString(courses["end_addr"]),
                            Gm_course = Convert.ToString(courses["gm_course"]),
                            Gm_text = Convert.ToString(courses["gm_text"])

                        });
                    }
                    this.DataContext = GalMaetgilInfo;  // 데이터 넘어오는지 확인
                    StsResult.Content = $"OpenAPI {GalMaetgilInfo.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"JSON 처리 오류 {ex.Message}");
            }
        }            

        // 실제 검색 메서드
        private async void SearchCourse(string courseName)
        {            // API 요청
            try
            {
                req = WebRequest.Create(openApiUri);    // URL을 넣어서 객체를 생성 
                res = await req.GetResponseAsync();        // 요청한 결과를 비동기 응답에 할당
                reader = new StreamReader(res.GetResponseStream());
                result = reader.ReadToEnd();    // json 결과 텍스트로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                reader.Close();
                res.Close();
            }
            // result를 json으로 변경
            var jsonResult = JObject.Parse(result); // string->json

            var total = Convert.ToInt32(jsonResult["total_results"]); // 전체 검색결과 수

            // await Commons.ShowMessageAsync("검색결과", total.ToString());   

            var items = jsonResult["results"];

            // items를 데이터 그리드에 표시
            var json_array = items as JArray;

            var courseItems = new List<CourseInfo>(); // json에서 넘어온 배열을 담을 장소
            foreach (var val in json_array)
            {
                var courseItem = new CourseInfo()
                {
                    Seq = Convert.ToInt32(val["seq"]),
                    Course_nm = Convert.ToString(val["course_nm"]),
                    Gugan_nm = Convert.ToString(val["gugan_nm"]),
                    Gm_range = Convert.ToString(val["gm_range"]),
                    Gm_degree = Convert.ToString(val["gm_degree"]),
                    Start_pls = Convert.ToString(val["start_pls"]),
                    Start_addr = Convert.ToString(val["start_pls"]),
                    Middle_pls = Convert.ToString(val["middle_pls"]),
                    Middle_adr = Convert.ToString(val["middle_adr"]),
                    End_pls = Convert.ToString(val["end_pls"]),
                    End_addr = Convert.ToString(val["end_addr"]),
                    Gm_course = Convert.ToString(val["gm_course"]),
                    Gm_text = Convert.ToString(val["gm_text"])
                };
                courseItems.Add(courseItem);
            }

            this.DataContext = courseItems;
            isFavorite = false;
            StsResult.Content = $"OpenAPI {courseItems.Count} 건 조회완료";
        }
        // 검색결과 중에서 좋아하는 영화 저장
        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기에 추가할 코스를 선택하세요(복수선택 가능");
                return;
            }

            if (isFavorite)
            {
                await Commons.ShowMessageAsync("오류", "이미 즐겨찾기한 코스입니다.");
                return;
            }
            
            try
            {
                // DB 연결 확인
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {

                    if (conn.State == ConnectionState.Closed) conn.Open();
                    var query = @"INSERT INTO favoritecourses
                                                (Seq,
                                                Course_nm,
                                                Gugan_nm,
                                                Gm_range,
                                                Gm_degree,
                                                Start_pls,
                                                Start_addr,
                                                Middle_pls,
                                                Middle_adr,
                                                End_pls,
                                                End_addr,
                                                Gm_course,
                                                Gm_text)
                                                VALUES
                                                (@Seq,
                                                @Course_nm,
                                                @Gugan_nm,
                                                @Gm_range,
                                                @Gm_degree,
                                                @Start_pls,
                                                @Start_addr,
                                                @Middle_pls,
                                                @Middle_adr,
                                                @End_pls,
                                                @End_addr,
                                                @Gm_course,
                                                @Gm_text)";

                    var insRes = 0;
                    foreach (CourseInfo item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Seq", item.Seq);
                        cmd.Parameters.AddWithValue("@Course_nm", item.Course_nm);
                        cmd.Parameters.AddWithValue("@Gugan_nm", item.Gugan_nm);
                        cmd.Parameters.AddWithValue("@Gm_range", item.Gm_range);
                        cmd.Parameters.AddWithValue("@Gm_degree", item.Gm_degree);
                        cmd.Parameters.AddWithValue("@Start_pls", item.Start_pls);
                        cmd.Parameters.AddWithValue("@Start_addr", item.Start_addr);
                        cmd.Parameters.AddWithValue("@Middle_pls", item.Middle_pls);
                        cmd.Parameters.AddWithValue("@Middle_adr", item.Middle_adr);
                        cmd.Parameters.AddWithValue("@End_pls", item.End_pls);
                        cmd.Parameters.AddWithValue("@End_addr", item.End_addr);
                        cmd.Parameters.AddWithValue("@Gm_course", item.Gm_course);
                        cmd.Parameters.AddWithValue("@Gm_text", item.Gm_text);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if (GrdResult.SelectedItems.Count == insRes)
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장성공");
                        StsResult.Content = $"즐겨찾기 {insRes} 건 저장완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("저장", "DB저장오류 관리자에게 문의하세요");
                    }
                }
            }
            catch (Exception ex)
            {

                await Commons.ShowMessageAsync("오류", $"DB저장 오류{ex.Message}");
            }
        }

        private void BtnMoveFacilities_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Owner = this;
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mainWindow.ShowDialog();
        }

        private void GrdResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 선택한 셀이 하나인 경우에만 실행
            if (GrdResult.SelectedItems.Count == 1)
            {
                // 선택한 셀의 내용 가져오기
                var cellInfo = GrdResult.SelectedItem as CourseInfo;
                var content = cellInfo.Gm_text;

                Informations.Text = content;
            }
        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            this.DataContext = null;

            List<CourseInfo> list = new List<CourseInfo>();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = @"SELECT 
                                    Seq,
                                    Course_nm,
                                    Gugan_nm,
                                    Gm_range,
                                    Gm_degree,
                                    Start_pls,
                                    Start_addr,
                                    Middle_pls,
                                    Middle_adr,
                                    End_pls,
                                    End_addr,
                                    Gm_course,
                                    Gm_text
                                FROM
                                    favoritecourses
                                      ORDER BY Seq ASC";
                    var cmd = new MySqlCommand(query, conn);
                    var adapter = new MySqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "FavoriteCourses");

                    foreach (DataRow dr in dSet.Tables["favoritecourses"].Rows)
                    {
                        list.Add(new CourseInfo
                        {
                            Seq = Convert.ToInt32(dr["seq"]),
                            Course_nm = Convert.ToString(dr["course_nm"]),
                            Gugan_nm = Convert.ToString(dr["gugan_nm"]),
                            Gm_range = Convert.ToString(dr["gm_range"]),
                            Gm_degree = Convert.ToString(dr["gm_degree"]),
                            Start_pls = Convert.ToString(dr["start_pls"]),
                            Start_addr = Convert.ToString(dr["start_pls"]),
                            Middle_pls = Convert.ToString(dr["middle_pls"]),
                            Middle_adr = Convert.ToString(dr["middle_adr"]),
                            End_pls = Convert.ToString(dr["end_pls"]),
                            End_addr = Convert.ToString(dr["end_addr"]),
                            Gm_course = Convert.ToString(dr["gm_course"]),
                            Gm_text = Convert.ToString(dr["gm_text"])
                        });
                    }

                    this.DataContext = list;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {list.Count} 건 조회완료";
                }
            }
            catch (Exception ex)
            {

                await Commons.ShowMessageAsync("오류", $"DB조회 오류 {ex.Message}");
            }
        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            if (isFavorite == false)
            {
                await Commons.ShowMessageAsync("오류", "즐겨찾기만 삭제할 수 있습니다.");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await Commons.ShowMessageAsync("오류", "삭제할 코스를 선택하세요.");
                return;
            }

            try // 삭제
            {
                using (MySqlConnection conn = new MySqlConnection(Commons.myConnString))
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    var query = "DELETE FROM favoritecourses WHERE Seq = @Seq";
                    var delRes = 0;

                    foreach (CourseInfo item in GrdResult.SelectedItems)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@Seq", item.Seq);

                        delRes += cmd.ExecuteNonQuery();
                    }

                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제성공");
                        StsResult.Content = $"즐겨찾기 {delRes} 건 저장완료";
                    }
                    else
                    {
                        await Commons.ShowMessageAsync("삭제", "DB삭제일부성공");   // 발생할 일이 거의 전무
                    }
                }
            }
            catch (Exception ex)
            {
                await Commons.ShowMessageAsync("오류", $"DB삭제 오류 {ex.Message}");
            }

            BtnViewFavorite_Click(sender, e);   // 즐겨찾기 보기 이벤트핸들러를 한번실행
        }




        //    }
        //}
    }
}