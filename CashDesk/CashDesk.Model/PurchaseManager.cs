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

        public void AddProductToBasket(ProductStack productStack) {
            if (productStack.Amount > 0) {
                Basket.AddProduct(productStack.Product, 1);//add to busket
                ProductManager.PopProduct(productStack, 1); //pop from productManager
            }
        }

        public void RemoveFromBasket(ProductStack productStack) {
            Basket.RemoveProduct(productStack);
            ProductManager.ReturnProduct(productStack, productStack.Amount);
        }
    }
}
