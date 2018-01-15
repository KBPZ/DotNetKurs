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
            ProductsInAutomata = new ObservableCollection<ProductVM>(_manager.ProductManager.ProductsInAutomata.Select(ap => new ProductVM(ap, _manager)));
            Watch(_manager.ProductManager.ProductsInAutomata, ProductsInAutomata, p => p.ProductStack);

            ProductsInBasket = new ObservableCollection<ProductVM>(_manager.Basket.ProductsInBasket.Select(p => new ProductVM(p, _manager)));
            Watch(_manager.Basket.ProductsInBasket, ProductsInBasket, p => p.ProductStack);

            AdminProducts = new ObservableCollection<AdminProductVM>(_manager.ProductManager.ProductsInAutomata.Select(p => new AdminProductVM(p, _manager)));
            Watch(_manager.ProductManager.ProductsInAutomata, AdminProducts, p => p.ProductStack);

            UsersInBase = new ObservableCollection<User>(_manager.UserManager.UsersInBase.Select(ap => ap));
            Watch(_manager.UserManager.UsersInBase, UsersInBase, p => p);

            DeactiveteAllWindow();
            //PasswordWindow = Visibility.Visible;//Окно пароля
            AdminWindow = Visibility.Visible;//Окно Админа
            //BasketWindow = Visibility.Visible;//Окно Покупок

            //AddTest = new DelegateCommand(() => _manager.ProductManager.AddProduct("Чёрная смерть", 10000, 2));
            //ChangeTest = new DelegateCommand(() => _manager.ProductManager.ChangeProduct("Кофе","Кофе 3в1",30));
            //BuyTest = new DelegateCommand(() => _manager.ProductManager.BuyProduct("Чай", 5));

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
        }

        //public DelegateCommand AddTest { get; }
        //public DelegateCommand ChangeTest { get; }
        //public DelegateCommand BuyTest { get; }

        public string Password { get { return _password; } set { SetProperty(ref _password, value); } }
        private string _password;

        public string Login { get { return _login; } set { SetProperty(ref _login,value); } }
        private string _login;

        public DelegateCommand VerificateUser { get; }
        public DelegateCommand OpenProductBase { get; }
        public ObservableCollection<ProductVM> ProductsInAutomata { get; }
        public ObservableCollection<User> UsersInBase { get; }
        public ObservableCollection<ProductVM> ProductsInBasket { get; }
        public ObservableCollection<AdminProductVM> AdminProducts { get; }
        private static PurchaseManager _manager = new PurchaseManager();


        public AdminProductVM SelectedAdminProduct { get { return _selectedAdminProduct; }
            set { SetProperty(ref _selectedAdminProduct, value); TmpSelectedAdminProduct = new AdminProductVM(value.ProductStack, value.Manager); } }
        private AdminProductVM _selectedAdminProduct;

        public AdminProductVM TmpSelectedAdminProduct { get { return _tmpSelectedAdminProduct; } set { SetProperty(ref _tmpSelectedAdminProduct, value); } }
        private AdminProductVM _tmpSelectedAdminProduct;

        private DelegateCommand _removeProductCommand;
        public DelegateCommand RemoveContactCommand => _removeProductCommand ?? (_removeProductCommand = new DelegateCommand(RemoveProduct, CanRemoveProduct));

        private void RemoveProduct()
        {
            _manager.ProductManager.DeleteProduct(TmpSelectedAdminProduct.Name);
        }

        private bool CanRemoveProduct()
        {
            if (TmpSelectedAdminProduct == null)
                return false;
            return true;
        }

        private DelegateCommand _createProductCommand;
        public DelegateCommand CreateProductCommand => _createProductCommand ?? (_createProductCommand = new DelegateCommand(CreateProduct));
        private void CreateProduct()
        {
            _manager.ProductManager.AddProduct("Новый товар", 0, 0);
        }

        //функция синхронизации ReadOnly коллекции элементов модели и соответствующей коллекции VM, 
        //в конструкторы которых передается эти экземпляры модели, указываемые в делегате
        private static void Watch<T, T2>(ReadOnlyObservableCollection<T> collToWatch, ObservableCollection<T2> collToUpdate,
            Func<T2, object> modelProperty) {
            ((INotifyCollectionChanged)collToWatch).CollectionChanged += (s, a) => {
                if (a.OldItems?.Count == 1) collToUpdate.Remove(collToUpdate.First(mv => modelProperty(mv) == a.OldItems[0]));
                if (a.NewItems?.Count == 1) collToUpdate.Add((T2)Activator.CreateInstance(typeof(T2), (T)a.NewItems[0], _manager));   
            };
        }                
        
        private void DeactiveteAllWindow()
        {
            PasswordWindow = Visibility.Hidden;
            BasketWindow = Visibility.Hidden;
            AdminWindow = Visibility.Hidden;
            ProductBaseWindow = Visibility.Hidden;
            UserBaseWindow = Visibility.Hidden;
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

    }

    public class ProductVM : BindableBase
    {
        public ProductStack ProductStack { get; }

        public ProductVM(ProductStack productStack, PurchaseManager manager = null)
        {
            ProductStack = productStack;
            productStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Amount)); };

            if (manager != null)
            {
                /*
                BuyCommand = new DelegateCommand(() => {
                    manager.BuyProduct(ProductStack.Product);
                });

                //*/

                AddProductToBasketCommand = new DelegateCommand(() => {
                    manager.AddProductToBasket(ProductStack);
                });
                RemoveFromBasketCommand = new DelegateCommand(() => {
                    manager.RemoveFromBasket(ProductStack);
                });
                PushOneCommand = new DelegateCommand(() => {
                    manager.PushOne(ProductStack);
                });
                PullOneCommand = new DelegateCommand(() => {
                    manager.PullOne(ProductStack);
                });


            }

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

        public AdminProductVM(ProductStack productStack, PurchaseManager manager = null)
        {
            Manager = manager;
            ProductStack = productStack;
            productStack.PropertyChanged += (s, a) => { RaisePropertyChanged(nameof(Amount)); };

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
}
