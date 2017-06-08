using GalaSoft.MvvmLight.Command;
using Soccer.Classes;
using Soccer.Models;
using Soccer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soccer.ViewModels
{
    public class ChangePasswordViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private DataService dataService;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
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

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
        #endregion

        #region Constructor
        public ChangePasswordViewModel()
        {
            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();

            IsEnabled = true;
        }
        #endregion

        #region Commands
        public ICommand ChangePasswordCommand { get { return new RelayCommand(ChangePassword); } }

        private async void ChangePassword()
        {
            if (string.IsNullOrEmpty(CurrentPassword))
            {
                await dialogService.ShowMessage("Error", "You must enter the current password.");
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();

            if (mainViewModel.CurrentUser.Password != CurrentPassword)
            {
                await dialogService.ShowMessage("Error", "The current password doesn´t  match.");
                return;
            }

            if (string.IsNullOrEmpty(NewPassword))
            {
                await dialogService.ShowMessage("Error", "You must enter a new password.");
                return;
            }

            if (NewPassword == CurrentPassword)
            {
                await dialogService.ShowMessage("Error", "The current password is equal to new password.");
                return;
            }

            if (NewPassword.Length < 6)
            {
                await dialogService.ShowMessage("Error", "The new password must have at least 6 characters.");
                return;
            }

            if (string.IsNullOrEmpty(ConfirmPassword))
            {
                await dialogService.ShowMessage("Error", "You must enter a password confirm.");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await dialogService.ShowMessage("Error", "The new password and confirm does not match.");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var parameters = dataService.First<Parameter>(false);
            var user = dataService.First<User>(false);

            var request = new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                Email = user.Email,
                NewPassword = NewPassword,  
            };

            var response = await apiService.ChangePassword(parameters.URLBase, "/api", "/Users/ChangePassword",
                user.TokenType, user.AccessToken, request);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            await dialogService.ShowMessage("Confirm", "Your password has been changed successfully.");
            await navigationService.Back();
        }

        #endregion
    }
}
