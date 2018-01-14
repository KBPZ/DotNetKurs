using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace CashDesk.Model {
    public class Basket {

        public Basket() {
            //продукты пользователя в корзине
            ProductsInBasket = new ReadOnlyObservableCollection<ProductStack>(_productsInBasket);
        }

        internal void AddProduct(Product product, int count) {
            var stack = _productsInBasket.FirstOrDefault(b => b.Product == product);
            if (stack == null)
                _productsInBasket.Add(new ProductStack(product, count));
            else
                stack.Push(count);
        }
        internal bool RemoveProduct(ProductStack productStack) {
            if (_productsInBasket.Remove(productStack))
                return true;
            return false;
        }
        public ReadOnlyObservableCollection<ProductStack> ProductsInBasket { get; }
        private readonly ObservableCollection<ProductStack> _productsInBasket = new ObservableCollection<ProductStack>();
    }


}
