using Soccer.Classes;
using Soccer.Models;
using Soccer.Pages;
using Soccer.Services;
using Soccer.ViewModels;
using System;

using Xamarin.Forms;

namespace Soccer
{
    public partial class App : Application
    {
        #region Attributes
        private DataService dataService;
        #endregion

        #region Properties
        public static NavigationPage Navigator { get; internal set; }

        public static MasterPage Master { get; internal set; }
        #endregion

        #region Constructor
        public App()
        {
            InitializeComponent();

            dataService = new DataService();

            LoadParameters();

            var user = dataService.First<User>(false);
            if (user != null && user.IsRemembered && user.TokenExpires > DateTime.Now)
            {
                var favoriteTeam = dataService.Find<Team>(user.FavoriteTeamId, false);
                user.FavoriteTeam = favoriteTeam;
                var mainViewModel = MainViewModel.GetInstance();
                mainViewModel.CurrentUser = user;
                mainViewModel.RegisterDevice();
                MainPage = new MasterPage();
            }
            else
            {
                MainPage = new LoginPage();
            }
        }
        #endregion

        #region Methods
        public static Action HideLoginView
        {
            get
            {
                return new Action(() => App.Current.MainPage = new LoginPage());
            }
        }

        public static async void NavigateToProfile(FacebookResponse profile)
        {
            var apiService = new ApiService();
            var dialogService = new DialogService();
            var navigationService = new NavigationService();
            var dataService = new DataService();

            var parameters = dataService.First<Parameter>(false);
            var token = await apiService.LoginFacebook(
                parameters.URLBase, "/api", "/Users/LoginFacebook", profile);
            if (token == null)
            {
                App.Current.MainPage = new LoginPage();
                return;
            }

            var response = await apiService.GetUserByEmail(
                parameters.URLBase,
                "/api",
                "/Users/GetUserByEmail",
                token.TokenType,
                token.AccessToken,
                token.UserName);

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", "Problem ocurred retrieving user information, try latter.");
                return;
            }

            var user = (User)response.Result;
            user.AccessToken = token.AccessToken;
            user.TokenType = token.TokenType;
            user.TokenExpires = token.Expires;
            user.IsRemembered = true;
            user.Password = profile.Id;
            dataService.DeleteAllAndInsert(user.FavoriteTeam);
            dataService.DeleteAllAndInsert(user.UserType);
            dataService.DeleteAllAndInsert(user);

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.CurrentUser = user;
            mainViewModel.RegisterDevice();
            navigationService.SetMainPage("MasterPage");
        }

        private void LoadParameters()
        {
            var urlBase = Application.Current.Resources["URLBase"].ToString();
            var urlBase2 = Application.Current.Resources["URLBase2"].ToString();
            var parameter = dataService.First<Parameter>(false);
            if (parameter == null)
            {
                parameter = new Parameter
                {
                    URLBase = urlBase,
                    URLBase2 = urlBase2,
                };

                dataService.Insert(parameter);
            }
            else
            {
                parameter.URLBase = urlBase;
                parameter.URLBase2 = urlBase2;
                dataService.Update(parameter);
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        #endregion
    }
}
