using GalaSoft.MvvmLight.Command;
using Plugin.Connectivity;
using Soccer.Models;
using Soccer.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Soccer.ViewModels
{
    public class SelectUserGroupViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private DataService dataService;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<UserGroupItemViewModel> UserGroups { get; set; }

        public bool IsRefreshing
        {
            set
            {
                if (isRefreshing != value)
                {
                    isRefreshing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
                }
            }
            get
            {
                return isRefreshing;
            }
        }
        #endregion

        #region Constructor
        public SelectUserGroupViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();

            UserGroups = new ObservableCollection<UserGroupItemViewModel>();

            LoadUserGroups();
        }
        #endregion

        #region Methods
        private async void LoadUserGroups()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await dialogService.ShowMessage("Error", "Check you internet connection.");
                return;
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                await dialogService.ShowMessage("Error", "Check you internet connection.");
                return;
            }

            IsRefreshing = true;
            var parameters = dataService.First<Parameter>(false);
            var user = dataService.First<User>(false);
            var response = await apiService.Get<UserGroup>(
                parameters.URLBase, "/api", "/Groups", 
                user.TokenType, user.AccessToken, user.UserId);
            IsRefreshing = false;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            ReloadUserGroups((List<UserGroup>)response.Result);
        }

        private void ReloadUserGroups(List<UserGroup> userGroups)
        {
            UserGroups.Clear();
            foreach (var userGroup in userGroups)
            {
                UserGroups.Add(new UserGroupItemViewModel
                {
                    GroupId = userGroup.GroupId,
                    GroupUsers = userGroup.GroupUsers,
                    Logo = userGroup.Logo,
                    Name = userGroup.Name,
                    Owner = userGroup.Owner,
                    OwnerId = userGroup.OwnerId,
                });
            }
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get { return new RelayCommand(Refresh); }
        }

        private void Refresh()
        {
            LoadUserGroups();
        }
        #endregion
    }
}
