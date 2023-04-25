using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using wp10_employeesApp.ViewModels;

namespace wp10_employeesApp
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        // 시작한 후에 초기화 진행
        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            // base.OnStartup(sender, e);   // 부모 클래스 실행은 주석처리
            await DisplayRootViewForAsync<MainViewModel>();
        }
    }
}
