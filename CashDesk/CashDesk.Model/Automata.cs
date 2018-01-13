using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CashDesk.Model
{
    public class Automata  : BindableBase
    {
        public Automata()
        {
            _productsInAutomata =
                new ObservableCollection<ProductStack>(Product.Products.Select(p => new ProductStack(p, 100)));
            ProductsInAutomata = new ReadOnlyObservableCollection<ProductStack>(_productsInAutomata);
        }

        public ReadOnlyObservableCollection<ProductStack> ProductsInAutomata { get; }

        private readonly ObservableCollection<ProductStack> _productsInAutomata;
    }
}