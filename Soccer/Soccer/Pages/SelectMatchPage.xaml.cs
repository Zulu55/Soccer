using Soccer.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Soccer.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectMatchPage : ContentPage
    {
        public SelectMatchPage()
        {
            InitializeComponent();

            var vm = SelectMatchViewModel.GetInstance();
            base.Appearing += (object sender, EventArgs e) =>
            {
                vm.RefreshCommand.Execute(this);
            };
        }
    }
}