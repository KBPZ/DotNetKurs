using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDesk.Model
{
    public class User : BindableBase
    {
        private User ap;
        private PurchaseManager _manager;

        internal static IReadOnlyList<User> BaseUsers => new List<User>()
                {
                    new User("Admin",UserType.Admin),
                    new User("User",UserType.Cashier)
                };

        public User(string name,UserType userType)
        {
            Name = name;
            UserType = userType;
        }

        /*
        public User(User ap, PurchaseManager manager)
        {
            this.ap = ap;
            _manager = manager;
        }*/

        public string Name { get; }
        public UserType UserType { get; }
    }

    public enum UserType : int
    {
        Cashier=0,
        Admin=1
    }

}
