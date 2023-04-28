using MahApps.Metro.Controls;
using System.Windows;

namespace wp12_finedustCheck
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

        public MapWindow(double coordy, double coordx) : this()
        {
            BrsLocSensor.Address = $"https://map.google.com/maps/place/{coordy},{coordx}";
        }
    }
}
