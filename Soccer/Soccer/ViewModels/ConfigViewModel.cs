﻿using System.Collections.Generic;
using System.Linq;
using Soccer.Models;
using System.ComponentModel;
using Soccer.Services;
using Xamarin.Forms;
using Plugin.Media.Abstractions;
using System.Collections.ObjectModel;
using Plugin.Connectivity;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Soccer.Classes;
using Plugin.Media;
using System;

namespace Soccer.ViewModels
{
    public class ConfigViewModel : User, INotifyPropertyChanged
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
        private bool allowToModify;
        private List<League> leagues;
        private int favoriteLeagueId;
        private int favoriteTeamId;
        private ImageSource imageSource;
        private MediaFile file;
        private User currentUser;
        #endregion

        #region Properties
        public ObservableCollection<LeagueItemViewModel> Leagues { get; set; }

        public ObservableCollection<TeamItemViewModel> Teams { get; set; }

        public int FavoriteLeagueId
        {
            set
            {
                if (favoriteLeagueId != value)
                {
                    favoriteLeagueId = value;
                    ReloadTeams(favoriteLeagueId);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FavoriteLeagueId"));
                }
            }
            get
            {
                return favoriteLeagueId;
            }
        }

        public new int FavoriteTeamId
        {
            set
            {
                if (favoriteTeamId != value)
                {
                    favoriteTeamId = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FavoriteTeamId"));
                }
            }
            get
            {
                return favoriteTeamId;
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

        public bool AllowToModify
        {
            set
            {
                if (allowToModify != value)
                {
                    allowToModify = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AllowToModify"));
                }
            }
            get
            {
                return allowToModify;
            }
        }

        public ImageSource ImageSource
        {
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                }
            }
            get
            {
                return imageSource;
            }
        }
        #endregion

        #region Constructor
        public ConfigViewModel(User currentUser)
        {
            this.currentUser = currentUser;

            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();

            Leagues = new ObservableCollection<LeagueItemViewModel>();
            Teams = new ObservableCollection<TeamItemViewModel>();

            FirstName = currentUser.FirstName;
            LastName = currentUser.LastName;
            Picture = currentUser.Picture;
            Email = currentUser.Email;
            NickName = currentUser.NickName;

            IsEnabled = true;
            AllowToModify = currentUser.UserTypeId == 1;

            LoadLeagues();
        }
        #endregion

        #region Methods
        private async void LoadLeagues()
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

            IsRunning = true;
            IsEnabled = false;

            var parameters = dataService.First<Parameter>(false);
            var response = await apiService.Get<League>(parameters.URLBase, "/api", "/Leagues");

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            leagues = (List<League>)response.Result;
            ReloadLeagues(leagues);
            ReloadTeams(currentUser.FavoriteTeam.LeagueId);

            FavoriteLeagueId = currentUser.FavoriteTeam.LeagueId;
            FavoriteTeamId = currentUser.FavoriteTeamId;
        }

        private void ReloadLeagues(List<League> leagues)
        {
            Leagues.Clear();
            foreach (var league in leagues.OrderBy(l => l.Name))
            {
                Leagues.Add(new LeagueItemViewModel
                {
                    LeagueId = league.LeagueId,
                    Logo = league.Logo,
                    Name = league.Name,
                    Teams = league.Teams,
                });
            }
        }

        private void ReloadTeams(int favoriteLeagueId)
        {
            var teams = leagues.Where(l => l.LeagueId == favoriteLeagueId).FirstOrDefault().Teams;
            Teams.Clear();
            foreach (var team in teams.OrderBy(t => t.Name))
            {
                Teams.Add(new TeamItemViewModel
                {
                    Fans = team.Fans,
                    Initials = team.Initials,
                    LeagueId = team.LeagueId,
                    Logo = team.Logo,
                    Name = team.Name,
                    TeamId = team.TeamId,
                });
            }
        }
        #endregion

        #region Commands
        public ICommand ChangePasswordCommand { get { return new RelayCommand(ChangePassword); } }

        private async void ChangePassword()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.ChangePassword = new ChangePasswordViewModel();
            await navigationService.Navigate("ChangePasswordPage");
        }

        public ICommand SaveCommand { get { return new RelayCommand(Save); } }

        private async void Save()
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                await dialogService.ShowMessage("Error", "You must enter a first name.");
                return;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                await dialogService.ShowMessage("Error", "You must enter a last name.");
                return;
            }

            if (string.IsNullOrEmpty(Email))
            {
                await dialogService.ShowMessage("Error", "You must enter a email.");
                return;
            }

            if (string.IsNullOrEmpty(NickName))
            {
                await dialogService.ShowMessage("Error", "You must enter a nick name.");
                return;
            }

            if (FavoriteTeamId == 0)
            {
                await dialogService.ShowMessage("Error", "You must select a favorite team.");
                return;
            }

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

            IsRunning = true;
            IsEnabled = false;

            byte[] imageArray = null;
            if (file != null)
            {
                imageArray = FilesHelper.ReadFully(file.GetStream());
                file.Dispose();
            }

            var user = new User
            {
                Email = Email,
                FavoriteTeamId = FavoriteTeamId,
                FirstName = FirstName,
                ImageArray = imageArray,
                LastName = LastName,
                NickName = NickName,
                Password = Password,
                UserTypeId = 1,
                UserId = currentUser.UserId,
            };

            var parameters = dataService.First<Parameter>(false);
            var response = await apiService.Put(parameters.URLBase, "/api", "/Users", 
                currentUser.TokenType, currentUser.AccessToken, user);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            response = await apiService.GetUserByEmail(
                parameters.URLBase,
                "/api",
                "/Users/GetUserByEmail",
                currentUser.TokenType,
                currentUser.AccessToken,
                Email);

            var newUser = (User)response.Result;
            newUser.AccessToken = currentUser.AccessToken;
            newUser.TokenType = currentUser.TokenType;
            newUser.TokenExpires = currentUser.TokenExpires;
            newUser.IsRemembered = currentUser.IsRemembered;
            newUser.Password = currentUser.Password;
            dataService.DeleteAllAndInsert(newUser.FavoriteTeam);
            dataService.DeleteAllAndInsert(newUser.UserType);
            dataService.DeleteAllAndInsert(newUser);

            IsRunning = false;
            IsEnabled = true;

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.CurrentUser = newUser;
            await navigationService.Back();
        }

        public ICommand CancelCommand { get { return new RelayCommand(Cancel); } }

        private void Cancel()
        {
            navigationService.SetMainPage("LoginPage");
        }

        public ICommand TakePictureCommand { get { return new RelayCommand(TakePicture); } }

        private async void TakePicture()
        {
            if (!AllowToModify)
            {
                return;
            }

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await dialogService.ShowMessage("No Camera", ":( No camera available.");
                return;
            }

            IsRunning = true;

            file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Directory = "Sample",
                Name = "test.jpg",
                PhotoSize = PhotoSize.Small,
            });

            if (file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }

            IsRunning = false;
        }
        #endregion
    }
}
