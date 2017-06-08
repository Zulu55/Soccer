using GalaSoft.MvvmLight.Command;
using Plugin.Connectivity;
using Soccer.Models;
using Soccer.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Linq;

namespace Soccer.ViewModels
{
    public class MyResultsViewModel : INotifyPropertyChanged
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
        private int tournamentGroupId;
        private string filter;
        private List<Result> results;
        #endregion

        #region Properties
        public ObservableCollection<ResultItemViewModel> Results { get; set; }

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

        public string Filter
        {
            set
            {
                if (filter != value)
                {
                    filter = value;
                    if (string.IsNullOrEmpty(filter))
                    {
                        ReloadResults(results);
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Filter"));
                }
            }
            get
            {
                return filter;
            }
        }
        #endregion

        #region Constructor
        public MyResultsViewModel(int tournamentGroupId)
        {
            this.tournamentGroupId = tournamentGroupId;

            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();

            Results = new ObservableCollection<ResultItemViewModel>();

            LoadResults();
        }
        #endregion

        #region Methods
        private async void LoadResults()
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
            var controller = string.Format("/Tournaments/GetResults/{0}/{1}", tournamentGroupId, user.UserId);
            var response = await apiService.Get<Result>(
                parameters.URLBase, "/api", controller, user.TokenType, user.AccessToken);
            IsRefreshing = false;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            results = (List<Result>)response.Result;
            ReloadResults(results);
        }

        private void ReloadResults(List<Result> results)
        {
            Results.Clear();
            foreach (var result in results)
            {
                Results.Add(new ResultItemViewModel
                {
                    LocalGoals = result.LocalGoals,
                    Match = result.Match,
                    MatchId = result.MatchId,
                    Points = result.Points,
                    PredictionId = result.PredictionId,
                    UserId = result.UserId,
                    VisitorGoals = result.VisitorGoals,
                });
            }
        }
        #endregion

        #region Commands
        public ICommand SearchResultCommand { get { return new RelayCommand(SearchResult); } }

        private void SearchResult()
        {
            var list = results
                .Where(r => r.Match.Local.Initials.ToUpper() == Filter.ToUpper() ||
                            r.Match.Visitor.Initials.ToUpper() == Filter.ToUpper())
                .ToList();
            ReloadResults(list);
        }

        public ICommand RefreshCommand { get { return new RelayCommand(Refresh); } }

        private void Refresh()
        {
            LoadResults();
        }
        #endregion
    }
}
