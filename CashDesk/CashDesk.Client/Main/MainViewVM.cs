using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using CashDesk.Model;
using CashDesk.Client.Main;

namespace Vending.Client.Main
{
    public class MainViewVM : BindableBase
    {
        public MainViewVM()
        {
            ProductsInAutomata = new ObservableCollection<ProductVM>(_manager.ProductManager.ProductsInAutomata.Select(ap => new ProductVM(ap, _manager, this)));
            Watch(this, _manager.ProductManager.ProductsInAutomata, ProductsInAutomata, p => p.ProductStack);

            ProductsInBasket = new ObservableCollection<ProductVM>(_manager.Basket.ProductsInBasket.Select(p => new ProductVM(p, _manager, this)));
            Watch(this, _manager.Basket.ProductsInBasket, ProductsInBasket, p => p.ProductStack);
            
            AdminProducts = new ObservableCollection<ProductVM>(_manager.ProductManager.ProductsInAutomata.Select(p => new ProductVM(p, _manager)));
            Watch(this, _manager.ProductManager.ProductsInAutomata, AdminProducts, p => p.ProductStack);

<<<<<<< HEAD
            UsersInBase = new ObservableCollection<UserVM>(_manager.UserManager.UsersInBase.Select(ap => new UserVM(ap,_manager)));
            Watch(_manager.UserManager.UsersInBase, UsersInBase, p => p.User);
=======
            UsersInBase = new ObservableCollection<User>(_manager.UserManager.UsersInBase.Select(ap => ap));
            Watch(this, _manager.UserManager.UsersInBase, UsersInBase, p => p);
>>>>>>> 796a565b79c251161315a92e11f773583d4d68aa

            DeactiveteAllWindow();
            PasswordWindow = Visibility.Visible;//Окно пароля
            //AdminWindow = Visibility.Visible;//Окно Админа
            //BasketWindow = Visibility.Visible;//Окно Покупок

            VerificateUser = new DelegateCommand(
                () => 
                {
                    if (_manager.UserManager.VerificateUser(Login, Password))
                    {
                        MessageBox.Show("Добро пожаловать");
                        var user = _manager.UserManager.GetUserByName(Login);
                        DeactiveteAllWindow();
                        if (user.UserType == UserType.Admin)
                            AdminWindow = Visibility.Visible;
                        else
                            BasketWindow = Visibility.Visible;
                    }
                    else
                        MessageBox.Show("Неправильное имя или пароль");
                });

            AllUserType= new ObservableCollection<UserType>();
            
            foreach(var type in Enum.GetNames(typeof(UserType)))
            {
                AllUserType.Add((UserType)Enum.Parse(typeof(UserType), type));
            }

            NewUser = new DelegateCommand(() => 
            {
                _manager.UserManager.AddUser("New User", UserType.Cashier, "1234");
            });
            DeleteUser = new DelegateCommand(() => 
            {
                if(FocuseUser!=null)
                    _manager.UserManager.DeleteUser(FocuseUser.Name);
            });

            LoginOut = new DelegateCommand(() =>
            {
                DeactiveteAllWindow();
                PasswordWindow = Visibility.Visible;
            });
        }

        public string Password { get { return _password; } set { SetProperty(ref _password, value); } }
        private string _password;

        public string Login { get { return _login; } set { SetProperty(ref _login,value); } }
        private string _login;


        public DelegateCommand LoginOut { get; }
        public DelegateCommand VerificateUser { get; }
        public DelegateCommand OpenProductBase { get; }
        public ObservableCollection<ProductVM> ProductsInAutomata { get; }
        public ObservableCollection<UserVM> UsersInBase { get; }
        public ObservableCollection<ProductVM> ProductsInBasket { get; }
        public ObservableCollection<ProductVM> AdminProducts { get; }
        private static PurchaseManager _manager = new PurchaseManager();

        public double _finalPrice;
        public double FinalPrice {
            get { return _finalPrice; }
            set { SetProperty(ref _finalPrice, value); }
        }

        public ProductVM SelectedAdminProduct
        { get
            { return _selectedAdminProduct; }
            set
            {
                SetProperty(ref _selectedAdminProduct, value);
                if(value!=null)
                    TmpSelectedAdminProduct = new AdminProductVM(value, value.Manager);
            }
        }
        private ProductVM _selectedAdminProduct;

        public AdminProductVM TmpSelectedAdminProduct { get { return _tmpSelectedAdminProduct; } set { SetProperty(ref _tmpSelectedAdminProduct, value); } }
        private AdminProductVM _tmpSelectedAdminProduct;

        private void RemoveProduct()
        {
            if (TmpSelectedAdminProduct != null)
                _manager.ProductManager.DeleteProduct(TmpSelectedAdminProduct.Name);
        }

        private DelegateCommand _createProductCommand;
        public DelegateCommand CreateProductCommand => _createProductCommand ?? (_createProductCommand = new DelegateCommand(CreateProduct));
        private void CreateProduct() => _manager.ProductManager.AddProduct("Новый товар", 0, 0);

        //функция синхронизации ReadOnly коллекции элементов модели и соответствующей коллекции VM, 
        //в конструкторы которых передается эти экземпляры модели, указываемые в делегате
        private static void Watch<T, T2, T3>(T3 self, ReadOnlyObservableCollection<T> collToWatch, ObservableCollection<T2> collToUpdate,
            Func<T2, object> modelProperty) {
            ((INotifyCollectionChanged)collToWatch).CollectionChanged += (s, a) => {
                if (a.OldItems?.Count == 1) collToUpdate.Remove(collToUpdate.First(mv => modelProperty(mv) == a.OldItems[0]));
                if (a.NewItems?.Count == 1) collToUpdate.Add((T2)Activator.CreateInstance(typeof(T2), (T)a.NewItems[0], _manager, self));   
            };
        }                
        
        private void DeactiveteAllWindow()
        {
            PasswordWindow = Visibility.Hidden;
            BasketWindow = Visibility.Hidden;
            AdminWindow = Visibility.Hidden;
            ProductBaseWindow = Visibility.Hidden;
            UserBaseWindow = Visibility.Hidden;
            ConfirmWindow = Visibility.Hidden;
        }

        private DelegateCommand _showProductBaseCommand;
        public DelegateCommand ShowProductBaseCommand => _showProductBaseCommand ?? (_showProductBaseCommand = new DelegateCommand(ShowProductBase));
        private void ShowProductBase()
        {
            DeactiveteAllWindow();
            ProductBaseWindow = Visibility.Visible;
        }

        private DelegateCommand _showUserBaseCommand;
        public DelegateCommand ShowUserBaseCommand => _showUserBaseCommand ?? (_showUserBaseCommand = new DelegateCommand(ShowUserBase));
        private void ShowUserBase()
        {
            DeactiveteAllWindow();
            UserBaseWindow = Visibility.Visible;
        }

        private DelegateCommand _showAdminWindowCommand;
        public DelegateCommand ShowAdminWindowCommand => _showAdminWindowCommand ?? (_showAdminWindowCommand = new DelegateCommand(ShowAdminWindow));
        private void ShowAdminWindow()
        {
            DeactiveteAllWindow();
            AdminWindow = Visibility.Visible;
        }

        private DelegateCommand _showConfirmWindowCommand;
        public DelegateCommand ShowConfirmWindowCommand => _showConfirmWindowCommand ?? (_showConfirmWindowCommand = new DelegateCommand(ShowConfirmWindow));
        private void ShowConfirmWindow()
        {
            DeactiveteAllWindow();
            ConfirmWindow = Visibility.Visible;
        }

        private DelegateCommand _confirmRemovalCommand;
        public DelegateCommand ConfirmRemovalCommand => _confirmRemovalCommand ?? (_confirmRemovalCommand = new DelegateCommand(ConfirmRemoval));
        private void ConfirmRemoval()
        {
            DeactiveteAllWindow();
            RemoveProduct();
            ProductBaseWindow = Visibility.Visible;
        }

        private DelegateCommand _denyRemovalCommand;
        public DelegateCommand DenyRemovalCommand => _denyRemovalCommand ?? (_denyRemovalCommand = new DelegateCommand(DenyRemoval));
        private void DenyRemoval()
        {
            DeactiveteAllWindow();
            ProductBaseWindow = Visibility.Visible;
        }

        public UserVM FocuseUser
        {
            get { return _focuseUser; }
            set
            {
                SetProperty(ref _focuseUser, value);
                if (value != null)
                {
                    TmpFocuseUser = new ChangebleUserVM(value.User, value.Manager);
                    FocusedType = value.UserType;
                }
            }
        }
        private UserVM _focuseUser;

        public ChangebleUserVM TmpFocuseUser { get => _tmpFocuseUser; set => SetProperty(ref _tmpFocuseUser, value); }
        private ChangebleUserVM _tmpFocuseUser;

        public ObservableCollection<UserType> AllUserType { get; }

        public UserType FocusedType
        {
            get => _focusedType;
            set
            {
                SetProperty(ref _focusedType, value);
                if (TmpFocuseUser != null)
                {
                    TmpFocuseUser.UserTypeValue = value;
                }
            }
        }
        private UserType _focusedType;

        public DelegateCommand NewUser { get; }
        public DelegateCommand DeleteUser { get; }


        public Visibility PasswordWindow { get { return _passwordWindow; } set { SetProperty(ref _passwordWindow, value); } }
        private Visibility _passwordWindow;
        public Visibility BasketWindow { get { return _basketWindow; } set { SetProperty(ref _basketWindow, value); } }
        private Visibility _basketWindow;
        public Visibility AdminWindow { get { return _adminWindow; } set { SetProperty(ref _adminWindow, value); } }
        private Visibility _adminWindow;
        public Visibility ProductBaseWindow { get { return _productBasWindow; } set { SetProperty(ref _productBasWindow, value); } }
        private Visibility _productBasWindow;
        public Visibility UserBaseWindow { get { return _userBaseWindow; } set { SetProperty(ref _userBaseWindow, value); } }
        private Visibility _userBaseWindow;
        public Visibility ConfirmWindow { get { return _confirmWindow; } set { SetProperty(ref _confirmWindow, value); } }
        private Visibility _confirmWindow;       

    }

    public class ProductVM : BindableBase
    {
        public ProductStack ProductStack { get { return _productStack; } set { SetProperty(ref _productStack, value); } }
        public ProductStack _productStack;
        public PurchaseManager Manager { get; }
        
        public ProductVM(ProductStack productStack, PurchaseManager manager = null, MainViewVM MVVM = null) {
            ProductStack = productStack;
            productStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Amount)); };
            productStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Price)); };
            productStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Name)); };

            if (manager != null && MVVM != null) {
                double debt;
                AddProductToBasketCommand = new DelegateCommand(() => {
                    manager.AddProductToBasket(ProductStack, out debt);
                    MVVM.FinalPrice = debt;
                });
                RemoveFromBasketCommand = new DelegateCommand(() => {
                    manager.RemoveFromBasket(ProductStack, out debt);
                    MVVM.FinalPrice = debt;
                });
                PushOneCommand = new DelegateCommand(() => {
                    manager.PushOne(ProductStack, out debt);
                    MVVM.FinalPrice = debt;
                });
                PullOneCommand = new DelegateCommand(() => {
                    manager.PullOne(ProductStack, out debt);
                    MVVM.FinalPrice = debt;
                });


            }
            Manager = manager;

        }

        public Visibility IsBuyVisible => BuyCommand == null ? Visibility.Collapsed : Visibility.Visible;
        public DelegateCommand BuyCommand { get; }
        public DelegateCommand AddProductToBasketCommand { get; }
        public DelegateCommand PushOneCommand { get; }
        public DelegateCommand PullOneCommand { get; }
        public DelegateCommand RemoveFromBasketCommand { get; }
        public string Name => ProductStack.Product.Name;
        public string Price => $"({ProductStack.Product.Price} руб.)";
        public int Amount => ProductStack.Amount;
    }


    public class AdminProductVM : BindableBase
    {
        public ProductStack ProductStack { get; }
        public PurchaseManager Manager { get; }

        public AdminProductVM(ProductVM productVM, PurchaseManager manager = null)
        {
            Manager = manager;
            ProductStack = productVM.ProductStack;
            productVM.ProductStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Amount)); };

            if (manager != null)
            {

            }

            SaveChanges = new DelegateCommand(() =>
            {
                if(Amount!= ProductStack.Amount)
                {
                    Manager.ProductManager.AddProductAmount(ProductStack.Product.Name, Amount - ProductStack.Amount);
                }

                if(Price!=ProductStack.Product.Price || Name != ProductStack.Product.Name)
                {
                    Manager.ProductManager.ChangeProduct(ProductStack.Product.Name, Name, Price);
                }

                ProductStack.Product.Name = _TmpName;
                ProductStack.Product.Price = _TmpPrice;
            });

            _TmpName = ProductStack.Product.Name;
            _TmpPrice = ProductStack.Product.Price;
            _TmpAmount = ProductStack.Amount;
        }

        public DelegateCommand SaveChanges { get; }

        public string Name { get { return _TmpName; } set { SetProperty(ref _TmpName, value); } }
        private string _TmpName;
        public int Price { get { return _TmpPrice; } set { SetProperty(ref _TmpPrice, value); } }
        private int _TmpPrice;
        public int Amount { get { return _TmpAmount; } set { SetProperty(ref _TmpAmount, value);} }
        private int _TmpAmount;
    }

    public class UserVM : BindableBase
    {
        public User User { get=>_user ; set=> SetProperty(ref _user,value); }
        private User _user;
        public PurchaseManager Manager { get; set; }

        public UserVM(User user, PurchaseManager manager)
        {
            user.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Name)); };
            user.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(UserType)); };
            User = user;
            Manager = manager;
        }
        public string Name => User.Name;
        public UserType UserType => User.UserType;
    }


    public class ChangebleUserVM : BindableBase
    {
        public User User { get; }
        public PurchaseManager Manager { get; }

        public ChangebleUserVM(User user, PurchaseManager manager)
        {
            user.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Name)); };
            user.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(UserTypeValue)); };
            User = user;
            Manager = manager;
            _name = User.Name;
            SaveChanges = new DelegateCommand(() =>
            {
                if(User.UserType!= UserTypeValue)
                    Manager.UserManager.ChangeUser(User.Name, UserTypeValue);
                if (User.Name != Name)
                    Manager.UserManager.ChangeUser(User.Name, Name);
                if (NewPassword != "")
                    Manager.UserManager.ChangePassword(User.Name, NewPassword);
            });

            ChangePassword = new DelegateCommand(()=> 
            {
                if (NewPassword != "")
                    Manager.UserManager.ChangePassword(User.Name, NewPassword);
            });
        }


        public DelegateCommand SaveChanges { get; }

        public DelegateCommand ChangePassword { get; }

        public string NewPassword { get => _newPassword; set => SetProperty(ref _newPassword, value); }
        private string _newPassword="";

        public string Name { get { return _name; } set { SetProperty(ref _name, value); } }
        private string _name;

        public UserType UserTypeValue { get => userTypeValue; set => userTypeValue = value; }
        private UserType userTypeValue;

    }
}
