using Soccer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Soccer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            var vm = MainViewModel.GetInstance();
            base.Appearing += (object sender, EventArgs e) =>
            {
                vm.RefreshPointsCommand.Execute(this);
            };
        }
    }
}
