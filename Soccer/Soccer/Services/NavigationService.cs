using Soccer.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soccer.Services
{
    public class NavigationService
    {
        #region Methods
        public void SetMainPage(string pageName)
        {
            switch (pageName)
            {
                case "MasterPage":
                    App.Current.MainPage = new MasterPage();
                    break;
                case "LoginPage":
                    App.Current.MainPage = new LoginPage();
                    break;
                case "NewUserPage":
                    App.Current.MainPage = new NewUserPage();
                    break;
                case "LoginFacebookPage":
                    App.Current.MainPage = new LoginFacebookPage();
                    break;
                case "ForgotPasswordPage":
                    App.Current.MainPage = new ForgotPasswordPage();
                    break;
                default:
                    break;
            }
        }

        public async Task Navigate(string pageName)
        {
            App.Master.IsPresented = false;

            switch (pageName)
            {
                case "SelectTournamentPage":
                    await App.Navigator.PushAsync(new SelectTournamentPage());
                    break;
                case "SelectMatchPage":
                    await App.Navigator.PushAsync(new SelectMatchPage());
                    break;
                case "EditPredictionPage":
                    await App.Navigator.PushAsync(new EditPredictionPage());
                    break;
                case "SelectGroupPage":
                    await App.Navigator.PushAsync(new SelectGroupPage());
                    break;
                case "PositionsPage":
                    await App.Navigator.PushAsync(new PositionsPage());
                    break;
                case "ConfigPage":
                    await App.Navigator.PushAsync(new ConfigPage());
                    break;
                case "ChangePasswordPage":
                    await App.Navigator.PushAsync(new ChangePasswordPage());
                    break;
                case "SelectUserGroupPage":
                    await App.Navigator.PushAsync(new SelectUserGroupPage());
                    break;
                case "UsersGroupPage":
                    await App.Navigator.PushAsync(new UsersGroupPage());
                    break;
                case "MyResultsPage":
                    await App.Navigator.PushAsync(new MyResultsPage());
                    break;
                default:
                    break;
            }
        }

        public async Task Back()
        {
            await App.Navigator.PopAsync();
        }
        #endregion
    }
}
