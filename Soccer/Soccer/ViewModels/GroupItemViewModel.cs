using GalaSoft.MvvmLight.Command;
using Soccer.Models;
using Soccer.Services;
using System.Windows.Input;

namespace Soccer.ViewModels
{
    public class GroupItemViewModel : Group
    {
        private NavigationService navigationService;
        private DataService dataService;

        public GroupItemViewModel()
        {
            navigationService = new NavigationService();
            dataService = new DataService();
        }

        public ICommand SelectGroupCommand { get { return new RelayCommand(SelectGroup); }  }

        private async void SelectGroup()
        {
            var mainViewModel = MainViewModel.GetInstance();
            var parameters = dataService.First<Parameter>(false);
            if (parameters.Option == "Tournaments")
            {
                mainViewModel.Positions = new PositionsViewModel(TournamentGroupId);
                await navigationService.Navigate("PositionsPage");
            }
            else
            {
                mainViewModel.MyResults = new MyResultsViewModel(TournamentGroupId);
                await navigationService.Navigate("MyResultsPage");
            }
        }
    }
}
