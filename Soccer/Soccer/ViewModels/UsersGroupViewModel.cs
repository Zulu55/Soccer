using Soccer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soccer.ViewModels
{
    public class UsersGroupViewModel : UserGroup
    {
        #region Atrributes
        private UserGroup userGroup;
        #endregion

        #region Properties
        public ObservableCollection<GroupUserItemViewModel> MyGroupUsers { get; set; }
        #endregion

        #region Constructor
        public UsersGroupViewModel(UserGroup userGroup)
        {
            this.userGroup = userGroup;

            GroupId = userGroup.GroupId;
            Name = userGroup.Name;
            Logo = userGroup.Logo;
            OwnerId = userGroup.OwnerId;
            Owner = userGroup.Owner;
            GroupUsers = userGroup.GroupUsers;

            MyGroupUsers = new ObservableCollection<GroupUserItemViewModel>();

            ReloadGroupUsers(GroupUsers);
        }
        #endregion

        #region Methods
        private void ReloadGroupUsers(List<GroupUser> groupUsers)
        {
            MyGroupUsers.Clear();
            foreach (var groupUser in groupUsers)
            {
                MyGroupUsers.Add(new GroupUserItemViewModel
                {
                    GroupId = groupUser.GroupId,
                    GroupUserId = groupUser.GroupUserId,
                    IsAccepted = groupUser.IsAccepted,
                    IsBlocked = groupUser.IsBlocked,
                    Points = groupUser.Points,
                    User = groupUser.User,
                    UserId = groupUser.UserId,
                });
            }
        }
        #endregion
    }
}
