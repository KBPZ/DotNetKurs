using Prism.Mvvm;

namespace CashDesk.Model
{
    public class ProductStack : BindableBase
    {
        private int _amount;

        public ProductStack(Product product, int amount)
        {
            Product = product;
            _amount = amount;
        }

        public Product Product { get; }

        public int Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        internal bool PullOne()
        {
            if (Amount > 0)
            {
                --Amount;
                return true;
            }
            return false;
        }

        internal void PushOne() => ++Amount;
    }
}