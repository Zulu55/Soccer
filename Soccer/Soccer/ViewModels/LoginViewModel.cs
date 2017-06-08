using GalaSoft.MvvmLight.Command;
using Plugin.Connectivity;
using Soccer.Models;
using Soccer.Services;
using System.ComponentModel;
using System.Windows.Input;
using System;

namespace Soccer.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private DataService dataService;
        private string email;
        private string password;
        private bool isRunning;
        private bool isEnabled;
        private bool isRemembered;
        #endregion

        #region Properties
        public string Email
        {
            set
            {
                if (email != value)
                {
                    email = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Email"));
                }
            }
            get
            {
                return email;
            }
        }

        public string Password
        {
            set
            {
                if (password != value)
                {
                    password = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Password"));
                }
            }
            get
            {
                return password;
            }
        }

        public bool IsRunning
        {
            set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRunning"));
                }
            }
            get
            {
                return isRunning;
            }
        }

        public bool IsEnabled
        {
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
                }
            }
            get
            {
                return isEnabled;
            }
        }

        public bool IsRemembered
        {
            set
            {
                if (isRemembered != value)
                {
                    isRemembered = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRemembered"));
                }
            }
            get
            {
                return isRemembered;
            }
        }
        #endregion

        #region Constructor
        public LoginViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();

            IsEnabled = true;
            IsRemembered = true;
        }
        #endregion

        #region Commands
        public ICommand ForgotPasswordCommand { get { return new RelayCommand(ForgotPassword); } }

        private void ForgotPassword()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.ForgotPassword = new ForgotPasswordViewModel();
            navigationService.SetMainPage("ForgotPasswordPage");
        }

        public ICommand LoginFacebookCommand { get { return new RelayCommand(LoginFacebook); } }

        private void LoginFacebook()
        {
            navigationService.SetMainPage("LoginFacebookPage");
        }

        public ICommand RegisterCommand { get { return new RelayCommand(Register); } }

        private void Register()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.NewUser = new NewUserViewModel();
            navigationService.SetMainPage("NewUserPage");
        }

        public ICommand LoginCommand { get { return new RelayCommand(Login); }  }

        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage("Error", "You must enter the user email.");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await dialogService.ShowMessage("Error", "You must enter a password.");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            if (!CrossConnectivity.Current.IsConnected)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Check you internet connection.");
                return;
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Check you internet connection.");
                return;
            }

            var parameters = dataService.First<Parameter>(false);
            var token = await apiService.GetToken(parameters.URLBase, Email, Password);

            if (token == null)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "The user name or password in incorrect.");
                Password = null;
                return;
            }

            if (string.IsNullOrEmpty(token.AccessToken))
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", token.ErrorDescription);
                Password = null;
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
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", "Problem ocurred retrieving user information, try latter.");
                return;
            }

            IsRunning = false;
            IsEnabled = true;

            var user = (User)response.Result;
            user.AccessToken = token.AccessToken;
            user.TokenType = token.TokenType;
            user.TokenExpires = token.Expires;
            user.IsRemembered = IsRemembered;
            user.Password = Password;
            dataService.DeleteAllAndInsert(user.FavoriteTeam);
            dataService.DeleteAllAndInsert(user.UserType);
            dataService.DeleteAllAndInsert(user);

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.CurrentUser = user;
            mainViewModel.RegisterDevice();
            navigationService.SetMainPage("MasterPage");

            Email = null;
            password = null;
        }
        #endregion
    }
}
