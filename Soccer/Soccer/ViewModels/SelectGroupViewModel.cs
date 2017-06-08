using System.Collections.Generic;
using Soccer.Models;
using System.Collections.ObjectModel;

namespace Soccer.ViewModels
{
    public class SelectGroupViewModel
    {
        #region Attributes
        private List<Group> groups;
        #endregion

        #region Properties
        public ObservableCollection<GroupItemViewModel> Groups { get; set; }
        #endregion

        #region Constructor
        public SelectGroupViewModel(List<Group> groups)
        {
            this.groups = groups;

            Groups = new ObservableCollection<GroupItemViewModel>();

            ReloadGroups(groups);
        }
        #endregion

        #region Methods
        private void ReloadGroups(List<Group> groups)
        {
            Groups.Clear();
            foreach (var group in groups)
            {
                Groups.Add(new GroupItemViewModel
                {
                    Name = group.Name,
                    TournamentGroupId = group.TournamentGroupId,
                    TournamentId = group.TournamentId,
                });
            }
        }
        #endregion
    }
}
