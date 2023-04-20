using System.Windows.Media;

namespace wp05_bikeshop.Logics
{
    internal class Car : Notifier
    {
        private string names;

        public string Names
        {
            get => names;
            set
            {
                names = value;
                OnPropertyChanged("Names");
            }
        }
        private double speed;
        public double Speed
        {
            get => speed;
            set
            {
                speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }
        public Color Colorz { get; set; }
        public Human Driver { get; set; }
    }
}
