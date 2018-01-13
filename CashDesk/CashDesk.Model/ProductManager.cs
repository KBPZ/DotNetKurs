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

        private XDocument xdoc = XDocument.Load(_nameFile);

        public ProductManager()
        {
            _productsInAutomata =
                ReadFormXml();
            ProductsInAutomata = new ReadOnlyObservableCollection<ProductStack>(_productsInAutomata);
        }

        public ReadOnlyObservableCollection<ProductStack> ProductsInAutomata { get; }
        private readonly ObservableCollection<ProductStack> _productsInAutomata;


        private void CreateXml()
        {
            XDocument xdoc = new XDocument();

            var products = xdoc.Element(_mainXElement);

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
        }

        public void AddProductAmount(String name,int amount)
        {
            var product = xdoc.Element("Products").Elements("Product").FirstOrDefault((x) => x.Attribute("Name").Value == name);

            var curentAmount = int.Parse(product.Element("Amount").Value);

            product.Element("Amount").Value = (curentAmount + amount).ToString();

            SaveInXml();
        }

        public void ChangeProduct(String name, String newName, int newPrice)
        {
            var product = xdoc.Element(_mainXElement).Elements(_productXElement).FirstOrDefault((x) => x.Attribute(_nameXAtribute).Value == name);

            if (product != null)
            {
                product.Attribute(_nameXAtribute).Value = newName;
                product.Element(_priceXElement).Value = newPrice.ToString();
            }
            SaveInXml();
        }

    private void SaveInXml()
        {
            xdoc.Save(_nameFile);
            ResetCollection();
        }

        private void ResetCollection()
        {
            _productsInAutomata.Clear();
            _productsInAutomata.AddRange(
                ReadFormXml());
        }
    }
}
