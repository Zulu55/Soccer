using GalaSoft.MvvmLight.Command;
using Soccer.Models;
using Soccer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Soccer.ViewModels
{
    public class MatchItemViewModel : Match
    {
        #region Attributes
        private NavigationService navigationService;
        #endregion

        #region Constructor
        public MatchItemViewModel()
        {
            navigationService = new NavigationService();
        }
        #endregion

        #region Commmands
        public ICommand SelectMatchCommand { get { return new RelayCommand(SelectMatch); } }

        private async void SelectMatch()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.EditPrediction = new EditPredictionViewModel(this);
            await navigationService.Navigate("EditPredictionPage");
        }
        #endregion
    }
}
