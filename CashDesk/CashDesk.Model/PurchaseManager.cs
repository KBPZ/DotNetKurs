using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDesk.Model
{
    public class PurchaseManager
    {        
        public Basket Basket { get; } = new Basket();
        public ProductManager ProductManager { get; } = new ProductManager();
        public UserManager UserManager { get; } = new UserManager();        

        public void BuyProduct(Product product)
        {
        }

        public void AddProductToBasket(ProductStack productStack, out double FinalPrice) { // добавить из меню всех товаров            
            if (ProductManager.PopProduct(productStack.Product, 1)) {
                FinalPrice = Basket.AddProduct(productStack.Product, 1);
                return;
            }
            FinalPrice = Basket.FinalPrice;
        }
        public void RemoveFromBasket(ProductStack productStack, out double FinalPrice) {//удалить из корзины
            FinalPrice = Basket.RemoveProduct(productStack);
            ProductManager.ReturnProduct(productStack.Product, productStack.Amount);
        }

        public void PushOne(ProductStack productStack, out double FinalPrice) { //добавить к количеству элемента, который находится в корзине из меню корзины
            if (ProductManager.PopProduct(productStack.Product, 1)) {
                FinalPrice = Basket.AddProduct(productStack.Product, 1);
                return;
            }
            FinalPrice = Basket.FinalPrice;
        }
        public void PullOne(ProductStack productStack, out double FinalPrice) { //отнять от элемента, который находится в корзине из меню корзины
            if (productStack.Amount > 0) {
                FinalPrice = Basket.PullProduct(productStack, 1);
                ProductManager.ReturnProduct(productStack.Product, 1);
                return;
            }
            FinalPrice = Basket.FinalPrice;
        }
    }
}
