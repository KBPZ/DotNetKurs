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
        public User()
        {
            //продукты пользователя
            UserBuyings = new ReadOnlyObservableCollection<ProductStack>(_userBuyings);
        }

        internal void AddProduct(Product product)
        {
            var stack = _userBuyings.FirstOrDefault(b => b.Product == product);
            if (stack == null)
                _userBuyings.Add(new ProductStack(product, 1));
            else
                stack.PushOne();
        }
        public ReadOnlyObservableCollection<ProductStack> UserBuyings { get; }
        private readonly ObservableCollection<ProductStack> _userBuyings = new ObservableCollection<ProductStack>();
    }
}
