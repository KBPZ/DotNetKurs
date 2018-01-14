using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDesk.Model
{
    public class PurchaseManager
    {        
        public Automata Automata { get; } = new Automata();
        public ProductManager ProductManager { get; } = new ProductManager();
        public UserManager UserManager { get; } = new UserManager();


        public void BuyProduct(Product product)
        {
        }
    }
}
