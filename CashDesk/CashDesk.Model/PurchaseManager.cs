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

        public void AddProductToBasket(ProductStack productStack) { // добавить из меню всех товаров
            if (ProductManager.PopProduct(productStack.Product, 1)) {
                Basket.AddProduct(productStack.Product, 1);
            }
        }

        public void RemoveFromBasket(ProductStack productStack) {//удалить из корзины
            Basket.RemoveProduct(productStack);
            ProductManager.ReturnProduct(productStack.Product, productStack.Amount);
        }

        public void PushOne(ProductStack productStack) { //добавить к количеству элемента, который находится в корзине из меню корзины
            if (ProductManager.PopProduct(productStack.Product, 1))
                Basket.AddProduct(productStack.Product, 1);                           
        }
        public void PullOne(ProductStack productStack) { //отнять от элемента, который находится в корзине из меню корзины
            if (productStack.Amount > 0) {
                Basket.PullProduct(productStack, 1);
                ProductManager.ReturnProduct(productStack.Product, 1);
            }
        }
    }
}
