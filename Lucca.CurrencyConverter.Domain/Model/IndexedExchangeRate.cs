namespace Lucca.CurrencyConverter.Domain.Model
{
    internal record IndexedExchangeRate(ExchangeRate ExchangeRate)
    {
        private IndexedExchangeRate? _linked;

        public IndexedExchangeRate(Currency from, Currency to, decimal changeRate, IndexedExchangeRate linked) : this(new ExchangeRate(from, to, changeRate))
        {
            _linked = linked;
        }

        public int Weight => (_linked?.Weight ?? 0) + 1;
    }
}
