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
    public class SelectMatchViewModel : INotifyPropertyChanged
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
        private int tournamentId;
        #endregion

        #region Properties
        public ObservableCollection<MatchItemViewModel> Matches { get; set; }

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
        public SelectMatchViewModel(int tournamentId)
        {
            instance = this;

            this.tournamentId = tournamentId;

            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();

            Matches = new ObservableCollection<MatchItemViewModel>();
        }
        #endregion

        #region Singleton
        private static SelectMatchViewModel instance;

        public static SelectMatchViewModel GetInstance()
        {
            return instance;
        }
        #endregion

        #region Methods
        private async void LoadMatches()
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
            var controller = string.Format("/Tournaments/GetMatchesToPredict/{0}/{1}", tournamentId, user.UserId);
            var response = await apiService.Get<Match>(
                parameters.URLBase, "/api", controller, user.TokenType, user.AccessToken);
            IsRefreshing = false;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }
            
            ReloadMatches((List<Match>)response.Result);
        }

        private void ReloadMatches(List<Match> matches)
        {
            Matches.Clear();
            foreach (var match in matches)
            {
                Matches.Add(new MatchItemViewModel
                {
                    DateId = match.DateId,
                    DateTime = match.DateTime,
                    Local = match.Local,
                    LocalGoals = match.LocalGoals,
                    LocalId = match.LocalId,
                    MatchId = match.MatchId,
                    StatusId = match.StatusId,
                    TournamentGroupId = match.TournamentGroupId,
                    Visitor = match.Visitor,
                    VisitorGoals = match.VisitorGoals,
                    VisitorId = match.VisitorId,
                    WasPredicted = match.WasPredicted,
                });
            }
        }

        #endregion

        #region Commands
        public ICommand RefreshCommand { get { return new RelayCommand(Refresh); }  }

        private void Refresh()
        {
            LoadMatches();
        }
        #endregion
    }
}
