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
    public class UserGroupItemViewModel : UserGroup
    {
        private NavigationService navigationService;

        public UserGroupItemViewModel()
        {
            navigationService = new NavigationService();
        }

        public ICommand SelectGroupCommand { get { return new RelayCommand(SelectGroup); }  }

        private async void SelectGroup()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.UsersGroup = new UsersGroupViewModel(this);
            await navigationService.Navigate("UsersGroupPage");
        }
    }
}
