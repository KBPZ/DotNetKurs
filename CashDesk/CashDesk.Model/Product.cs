using System.Collections.Generic;

namespace CashDesk.Model
{
    public class Product
    {
        public static IReadOnlyList<Product> Products = new List<Product>();

        internal Product(string name, int price)
        {
            Name = name;
            Price = price;
        }
        public string Name { get; }
        public int Price { get; }
    }
}