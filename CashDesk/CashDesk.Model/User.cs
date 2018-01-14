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
        internal static IReadOnlyList<User> BaseUsers
        {
            get
            {
                return new List<User>()
                {
                    new User("Admin",UserType.Admin),
                    new User("User",UserType.Cashier)
                };
            }
        }

        public User(string name,UserType userType)
        {
            Name = name;
            UserType = UserType;
        }

        public string Name { get; }
        public UserType UserType { get; }
    }

    public enum UserType : int
    {
        Cashier=0,
        Admin=1
    }

}
