using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace CashDesk.Model {
    public class Basket: BindableBase {

        public Basket() {
            //продукты пользователя в корзине
            ProductsInBasket = new ReadOnlyObservableCollection<ProductStack>(_productsInBasket);
            _finalPrice = 0;
        }

        internal double AddProduct(Product product, int count) {
            var stack = _productsInBasket.FirstOrDefault(b => b.Product == product);
            if (stack == null)
                _productsInBasket.Add(new ProductStack(product, count));
            else
                stack.Push(count);
            _finalPrice += product.Price;
            return _finalPrice;
        }
        internal double PullProduct(ProductStack productStack, int count) {
            var stack = _productsInBasket.FirstOrDefault(b => b.Product == productStack.Product);
            if (stack != null && productStack.Amount >= count) {
                stack.Pull(count);
                _finalPrice -= count * productStack.Product.Price;
            }
            return _finalPrice;
        }
        internal double RemoveProduct(ProductStack productStack) {
            double price = productStack.Product.Price * productStack.Amount;
            if (_productsInBasket.Remove(productStack)) {
                _finalPrice -= price;                
            }
            return _finalPrice;
        }
        internal void BuyProduct() {
            _productsInBasket.Clear();
            _finalPrice = 0;
        }

        public ReadOnlyObservableCollection<ProductStack> ProductsInBasket { get; }
        private readonly ObservableCollection<ProductStack> _productsInBasket = new ObservableCollection<ProductStack>();

        private double _finalPrice;
        public double FinalPrice {
            get { return _finalPrice; }
        }
    }


}
