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
    public class PositionsViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private DataService dataService;
        private bool isRefreshing;
        private int tournamentGroupId;
        #endregion

        #region Properties
        public ObservableCollection<TournamentTeamItemViewModel> TournamentTeams { get; set; }

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
        public PositionsViewModel(int tournamentGroupId)
        {
            this.tournamentGroupId = tournamentGroupId;

            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();

            TournamentTeams = new ObservableCollection<TournamentTeamItemViewModel>();

            LoadTournamentTeams();
        }
        #endregion

        #region Methods
        private async void LoadTournamentTeams()
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
            var response = await apiService.Get<TournamentTeam>(
                parameters.URLBase, "/api", "/TournamentTeams", user.TokenType, user.AccessToken, 
                tournamentGroupId);
            IsRefreshing = false;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            ReloadTournamentTeams((List<TournamentTeam>)response.Result);
        }

        private void ReloadTournamentTeams(List<TournamentTeam> tournamentTeams)
        {
            TournamentTeams.Clear();
            foreach (var tournamentTeam in tournamentTeams)
            {
                TournamentTeams.Add(new TournamentTeamItemViewModel
                {
                    AgainstGoals = tournamentTeam.AgainstGoals,
                    FavorGoals = tournamentTeam.FavorGoals,
                    MatchesLost = tournamentTeam.MatchesLost,
                    MatchesPlayed = tournamentTeam.MatchesPlayed,
                    MatchesTied = tournamentTeam.MatchesTied,
                    MatchesWon = tournamentTeam.MatchesWon,
                    Points = tournamentTeam.Points,
                    Position = tournamentTeam.Position,
                    Team = tournamentTeam.Team,
                    TeamId = tournamentTeam.TeamId,
                    TournamentGroupId = tournamentTeam.TournamentGroupId,
                    TournamentTeamId = tournamentTeam.TournamentTeamId,
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
            LoadTournamentTeams();
        }
        #endregion
    }
}
