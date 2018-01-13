using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDesk.Model
{
    public class PurchaseManager
    {        
        public User User { get; } = new User();
        public Automata Automata { get; } = new Automata();
        public ProductManager ProductManager { get; } = new ProductManager();


        public void BuyProduct(Product product)
        {
            if (Automata.BuyProduct(product))
                User.AddProduct(product);
        }
    }
}
