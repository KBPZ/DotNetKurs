using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace CashDesk.Model
{
    public class UserManager : BindableBase
    {

        private const string _nameFile = "Users.xml";
        private const string _mainXElement = "Users";
        private const string _usersXElement = "User";
        private const string _nameXAtribute = "Name";
        private const string _crioptoPassword = "CrioptoPassword";
        private const string _salt = "Salt";
        private const string _userType = "UserType";

        private XDocument xdoc;

        public UserManager()
        {
            try
            {
                xdoc = XDocument.Load(_nameFile);
            }
            catch (System.IO.FileNotFoundException)
            {
                CreateXml();
                xdoc = XDocument.Load(_nameFile);
            }
            //Test();
        }

        private void Test()
        {
            AddUser("Pavel", UserType.Cashier, "nya");
            var user = GetUserByName("Pavel");
            if (!VerificateUser("Pavel","nya"))
                return;
            AddUser("Chek1", UserType.Cashier, "da");
            ChangePassword("Pavel", "nihahaha");
            if (VerificateUser("Pavel", "nya"))
                return;
            if (!VerificateUser("Pavel", "nihahaha"))
                return;
            AddUser("Chek2", UserType.Cashier, "da");
            AddUser("NeDoAdmin", UserType.Cashier, "hey");
            ChangeUser("NeDoAdmin", UserType.Admin);
            ChangeUser("NeDoAdmin", "DoAdmin");
            
        }

        private void CreateXml()
        {
            XDocument xdoc = new XDocument();

            var users = new XElement(_mainXElement);

            foreach (var user in User.BaseUsers)
            {
                string salt = NewRandomSalt();
                users.Add(new XElement(_usersXElement,
                    new XAttribute(_nameXAtribute, user.Name),
                    new XElement(_userType, (int)user.UserType),
                    new XElement(_salt, salt),
                    new XElement(_crioptoPassword, Encrypting("1234" + salt)))
                    );
            }

            xdoc.Add(users);
            xdoc.Save(_nameFile);
        }

        public void AddUser(string userName, UserType userType, string password)
        {
            var users = xdoc.Element(_mainXElement);
            var user = xdoc.Element(_mainXElement).Elements(_usersXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == userName);

            if (user == null)
            {
                string salt = NewRandomSalt();
                users.Add(new XElement(_usersXElement,
                    new XAttribute(_nameXAtribute, userName),
                    new XElement(_userType, (int)userType),
                    new XElement(_salt, salt),
                    new XElement(_crioptoPassword, Encrypting(password + salt)))
                    );
            }
            SaveChanges();
        }

        public void ChangePassword(string name, string newPassword)
        {
            var user = xdoc.Element(_mainXElement).Elements(_usersXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);
            if (user != null)
            {
                string salt = NewRandomSalt();
                user.Element(_crioptoPassword).Value = Encrypting(newPassword + salt);
                user.Element(_salt).Value = salt;
            }
            SaveChanges();
        }

        public void DeleteUser(string name)
        {
            var users = xdoc.Element(_mainXElement);
            var user = xdoc.Element(_mainXElement).Elements(_usersXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);

            if (user != null)
                user.Remove();

            SaveChanges();
        }

        public void ChangeUser(string name, string newName)
        {
            var user = xdoc.Element(_mainXElement).Elements(_usersXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);
            if (user != null)
            {
                user.Attribute(_nameXAtribute).Value = newName;
            }
            SaveChanges();
        }

        public void ChangeUser(string name, UserType newUserType)
        {
            var user = xdoc.Element(_mainXElement).Elements(_usersXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);
            if (user != null)
            {
                user.Element(_userType).Value = ((int)newUserType).ToString();
            }
            SaveChanges();
        }

        private void SaveChanges()
        {
            xdoc.Save(_nameFile);
        }

        public bool VerificateUser(string name, string password)
        {
            var user = xdoc.Element(_mainXElement).Elements(_usersXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);
            if (user != null)
            {
                string salt = user.Element(_salt).Value;
                string criptoPass = user.Element(_crioptoPassword).Value;
                return Encrypting(password + salt) == criptoPass;
            }
            return false;
        }

        public User GetUserByName(string name)
        {
            var user = xdoc.Element(_mainXElement).Elements(_usersXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);
            if (user != null)
            {
                UserType userType = (UserType)int.Parse(user.Element(_userType).Value);
                return new User(name, userType); 
            }
            return null;
        }

        private string Encrypting(string word)
        {
            return word;
        }

        private string NewRandomSalt()
        {
            return "sflprt49fhi2";
        }
    }
}
