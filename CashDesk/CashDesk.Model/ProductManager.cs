using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using System.Xml.Linq;

namespace CashDesk.Model
{
    public class ProductManager : BindableBase
    {
        private const string _mainXElement = "Products";
        private const string _productXElement = "Product";
        private const string _nameFile = "products.xml";
        private const string _nameXAtribute = "Name";
        private const string _priceXElement = "Price";
        private const string _amountXElement = "Amount";

        private XDocument xdoc;

        public ProductManager()
        {
            try
            {
                xdoc = XDocument.Load(_nameFile);
            }
            catch (System.IO.FileNotFoundException)
            {
                CreateXml();
                xdoc= XDocument.Load(_nameFile);
            }


            _productsInAutomata =
                ReadFormXml();
            ProductsInAutomata = new ReadOnlyObservableCollection<ProductStack>(_productsInAutomata);
        }

        public ReadOnlyObservableCollection<ProductStack> ProductsInAutomata { get; }
        private readonly ObservableCollection<ProductStack> _productsInAutomata;


        private void CreateXml()
        {
            XDocument xdoc = new XDocument();

            var products = new XElement(_mainXElement);

            foreach (var productStack in Product.Products)
            {
                products.Add( new XElement(nameof(Product),
                    new XAttribute(_nameXAtribute, productStack.Name),
                    new XElement(_priceXElement, productStack.Price),
                    new XElement(_amountXElement, "100")));

            }
            xdoc.Add(products);
            xdoc.Save(_nameFile);
        }

        private ObservableCollection<ProductStack> ReadFormXml()
        {
            var ret = new ObservableCollection<ProductStack>();


            var products = xdoc.Element(_mainXElement);
            foreach(var productElement in xdoc.Element(_mainXElement).Elements(_productXElement))
            {
                var name = productElement.Attribute(_nameXAtribute);
                var amount = productElement.Element(_amountXElement);
                var price = productElement.Element(_priceXElement);
                if(name!=null&&amount!=null&&price!=null)
                {
                    ret.Add(new ProductStack(new Product(name.Value, int.Parse(price.Value)), int.Parse(amount.Value)));
                }
            }

            return ret;
        }

        public bool BuyProduct(String productName, int count)
        {
            var buyProduct = xdoc.Element(_mainXElement).Elements(_productXElement).FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value==productName);

            var amount = int.Parse(buyProduct.Element(_amountXElement).Value);

            if(count>amount)
                return false;

            buyProduct.Element(_amountXElement).Value = (amount - count).ToString();

            SaveInXml();
            return true;
        }
        
        /// //////////////
        public bool PopProduct(Product product, int count) {
            var stack = _productsInAutomata.FirstOrDefault(b => b.Product == product);
            if (stack == null || stack.Amount <= 0)
                return false;
            stack.Pull(count);
            return true;
        }

        public void ReturnProduct(Product product, int count) {
            var stack = _productsInAutomata.FirstOrDefault(b => b.Product == product);
            stack.Push(count);
            return;
        }

        /// //////////////
        
        public void BuyProducts()
        {
            var products = xdoc.Element(_mainXElement);
            foreach (var productStack in _productsInAutomata)
            {
                var product = products.Elements(_productXElement).FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == productStack.Product.Name);
                if (product != null)
                {
                    product.Element(_amountXElement).Value = productStack.Amount.ToString();
                }
            }
            SaveInXml();
        }

        public void AddProduct(String name, int price, int amount)
        {
            var products = xdoc.Element(_mainXElement);
            var product = xdoc.Element(_mainXElement).Elements(_productXElement).FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);

            if (product == null)
            {
                products.Add(new XElement(nameof(Product),
                    new XAttribute(_nameXAtribute, name),
                    new XElement(_priceXElement, price),
                    new XElement(_amountXElement, amount)));
            }
            SaveInXml();
            ResetCollection();
        }

        public void AddProductAmount(String name,int amount)
        {
            var product = xdoc.Element(_mainXElement).Elements(_productXElement).FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);



            if (product != null)
            {
                int curentAmount = int.Parse(product.Element(_amountXElement).Value);
                product.Element(_amountXElement).Value = (curentAmount + amount).ToString();
                var productInAutomata = _productsInAutomata.First(x => x.Product.Name == name);

                productInAutomata.Amount = (curentAmount + amount);
            }

            SaveInXml();
            //ResetCollection();
        }

        public void ChangeProduct(String name, String newName, int newPrice)
        {
            var product = xdoc.Element(_mainXElement).Elements(_productXElement).FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);

            if (product != null)
            {
                product.Attribute(_nameXAtribute).Value = newName;
                product.Element(_priceXElement).Value = newPrice.ToString();

                var productInAutomata = _productsInAutomata.First(x => x.Product.Name == name);
                productInAutomata.Product.Name = newName;
                productInAutomata.Product.Price = newPrice;

            }
            SaveInXml();
            //ResetCollection();
        }

        public void DeleteProduct(string name)
        {
            var product = xdoc.Element(_mainXElement).Elements(_productXElement)
                .FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);

            product.Remove();
            SaveInXml();
            ResetCollection();
        }

        private void SaveInXml()
        {
            xdoc.Save(_nameFile);
        }

        private void ResetCollection()
        {
            for(; _productsInAutomata.Any();)
            {
                _productsInAutomata.Remove(_productsInAutomata.First());
            }

            _productsInAutomata.AddRange(ReadFormXml());
        }
    }
}
