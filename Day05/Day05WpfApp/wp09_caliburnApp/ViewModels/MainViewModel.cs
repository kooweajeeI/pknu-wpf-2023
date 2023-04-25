using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using wp09_caliburnApp.Models;

namespace wp09_caliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        private string firstName = "jaewook";

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(() => FullName);
            }

        }

        private string lastName = "Lee";

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(() => FullName);
            }
        }

        public string FullName
        {
            get => $"{lastName} {FirstName}";
        }
        // 콤보박스에 바인딩할 속성
        // 이런곳에는 var사용 못함
        private BindableCollection<Person> managers = new BindableCollection<Person>();

        public BindableCollection<Person> Managers
        {
            get => managers;
            set => managers = value;
        }

        private Person selectedManager;

        public Person SelectedManager
        {
            get => selectedManager;
            set
            {
                selectedManager = value;
                LastName = SelectedManager.LastName;
                FirstName = SelectedManager.FirstName;
                NotifyOfPropertyChange(nameof(SelectedManager));
            }
        }

        public MainViewModel()
        {
            //DB를 사용하면 여기서 DB접속 > 데이터 select까지
            Managers.Add(new Person { FirstName = "John", LastName = "Carmack" });
            Managers.Add(new Person { FirstName = "Steve", LastName = "Jobs" });
            Managers.Add(new Person { FirstName = "Bill", LastName = "Gates" });
            Managers.Add(new Person { FirstName = "Elon", LastName = "Musk" });
        }

        public void ClearName()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public bool CanClearName
        {
            get => !(string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName));
        }
    }
}