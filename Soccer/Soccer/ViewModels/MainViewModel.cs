using GalaSoft.MvvmLight.Command;
using Soccer.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using Plugin.Connectivity;
using Soccer.Services;
using Xamarin.Forms;
using Soccer.Interfaces;

namespace Soccer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DataService dataService;
        private User currentUser;
        #endregion

        #region Properties
        public LoginViewModel Login { get; set; }

        public SelectTournamentViewModel SelectTournament { get; set; }

        public SelectMatchViewModel SelectMatch { get; set; }

        public EditPredictionViewModel EditPrediction { get; set; }

        public NewUserViewModel NewUser { get; set; }

        public SelectGroupViewModel SelectGroup { get; set; }

        public PositionsViewModel Positions { get; set; }

        public ConfigViewModel Config { get; set; }

        public ChangePasswordViewModel ChangePassword { get; set; }

        public SelectUserGroupViewModel SelectUserGroup { get; set; }

        public UsersGroupViewModel UsersGroup { get; set; }

        public ProfileViewModel Profile { get; set; }

        public ForgotPasswordViewModel ForgotPassword { get; set; }

        public MyResultsViewModel MyResults { get; set; }

        public User CurrentUser
        {
            set
            {
                if (currentUser != value)
                {
                    currentUser = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentUser"));
                }
            }
            get
            {
                return currentUser;
            }
        }

        public ObservableCollection<MenuItemViewModel> Menu { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            instance = this;

            apiService = new ApiService();
            dataService = new DataService();

            Login = new LoginViewModel();

            LoadMenu();
        }
        #endregion

        #region Singleton
        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                instance = new MainViewModel();
            }

            return instance;
        }
        #endregion

        #region Methods
        public void RegisterDevice()
        {
            var register = DependencyService.Get<IRegisterDevice>();
            register.RegisterDevice();
        }

        private void LoadMenu()
        {
            Menu = new ObservableCollection<MenuItemViewModel>();

            Menu.Add(new MenuItemViewModel
            {
                Icon = "predictions.png",
                PageName = "SelectTournamentPage",
                Title = "Predictions",
            });

            Menu.Add(new MenuItemViewModel
            {
                Icon = "groups.png",
                PageName = "SelectUserGroupPage",
                Title = "Groups",
            });

            Menu.Add(new MenuItemViewModel
            {
                Icon = "tournaments.png",
                PageName = "SelectTournamentPage",
                Title = "Tournaments",
            });

            Menu.Add(new MenuItemViewModel
            {
                Icon = "myresults.png",
                PageName = "SelectTournamentPage",
                Title = "My Results",
            });

            Menu.Add(new MenuItemViewModel
            {
                Icon = "config.png",
                PageName = "ConfigPage",
                Title = "Config",
            });

            Menu.Add(new MenuItemViewModel
            {
                Icon = "logut.png",
                PageName = "LoginPage",
                Title = "Logut",
            });
        }
        #endregion

        #region Commands
        public ICommand RefreshPointsCommand { get { return new RelayCommand(RefreshPoints) ; }  }

        private async void RefreshPoints()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return; // Do nichim
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                return; // Do nichim
            }

            var parameters = dataService.First<Parameter>(false);
            var user = dataService.First<User>(false);
            var response = await apiService.GetPoints(
                parameters.URLBase, "/api", "/Users/GetPoints", 
                user.TokenType, user.AccessToken, user.UserId);

            if (!response.IsSuccess)
            {
                return; // Do nichim
            }

            var point = (Models.Point)response.Result;
            if (CurrentUser.Points != point.Points)
            {
                CurrentUser.Points = point.Points;
                dataService.Update(CurrentUser);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentUser"));
            }
        }
        #endregion
    }
}
