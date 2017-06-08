using GalaSoft.MvvmLight.Command;
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
    public class EditPredictionViewModel : Match, INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private ApiService apiService;
        private DialogService dialogService;
        private NavigationService navigationService;
        private DataService dataService;
        private Match match;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties
        public string GoalsLocal { get; set; }

        public string GoalsVisitor { get; set; }

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
        #endregion

        #region Constructor
        public EditPredictionViewModel(Match match)
        {
            this.match = match;

            apiService = new ApiService();
            dialogService = new DialogService();
            navigationService = new NavigationService();
            dataService = new DataService();

            DateId = match.DateId;
            DateTime = match.DateTime;
            Local = match.Local;
            LocalGoals = match.LocalGoals;
            LocalId = match.LocalId;
            MatchId = match.MatchId;
            StatusId = match.StatusId;
            TournamentGroupId = match.TournamentGroupId;
            Visitor = match.Visitor;
            VisitorGoals = match.VisitorGoals;
            VisitorId = match.VisitorId;
            WasPredicted = match.WasPredicted;

            GoalsLocal = LocalGoals2.ToString();
            GoalsVisitor = VisitorGoals2.ToString();

            IsEnabled = true;
        }

        #endregion

        #region Commands
        public ICommand SaveCommand { get { return new RelayCommand(Save); }  }

        private async void Save()
        {
            if (string.IsNullOrEmpty(GoalsLocal))
            {
                await dialogService.ShowMessage("Error", "You must enter a valid local goals.");
                return;
            }

            if (string.IsNullOrEmpty(GoalsVisitor))
            {
                await dialogService.ShowMessage("Error", "You must enter a valid visitor goals.");
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            var parameters = dataService.First<Parameter>(false);
            var user = dataService.First<User>(false);

            var prediction = new Prediction
            {
                LocalGoals = int.Parse(GoalsLocal),
                MatchId = MatchId,
                Points = 0,
                UserId = user.UserId,
                VisitorGoals = int.Parse(GoalsVisitor),
            };

            var response = await apiService.Post(
                parameters.URLBase, "/api", "/Predictions", user.TokenType, user.AccessToken, 
                prediction);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage("Error", response.Message);
                return;
            }

            await navigationService.Back();
        }
        #endregion
    }
}
