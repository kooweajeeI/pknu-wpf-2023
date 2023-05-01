using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wp13_BusanGalmaetgilApp
{
    /// <summary>
    /// MapWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MapWindow : MetroWindow
    {
        public MapWindow()
        {
            InitializeComponent();
        }

        public MapWindow(double Lng, double Lat) : this()
        {
            BrsLocSensor.Address = $"https://map.google.com/maps/place/{Lng},{Lat}";
        }
    }
}
