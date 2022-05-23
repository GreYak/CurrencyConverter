namespace Lucca.CurrencyConverter.Domain.Model
{
    /// <summary>
    /// Represents an indexed exchange rate.
    /// </summary>
    /// <param name="ExchangeRate">The exchange to be indexed.</param>
    internal class IndexedExchangeRate
    {
        private readonly ExchangeRate _exchangeRate;
        private readonly IndexedExchangeRate? _previousIndexlinked;

        /// <summary>
        /// Initialize a new instance of <see cref="IndexedExchangeRate"/>
        /// </summary>
        public IndexedExchangeRate(ExchangeRate exchangeRate, IndexedExchangeRate? previousIndex = null)
        {
            _exchangeRate = exchangeRate;
            _previousIndexlinked = previousIndex;
        }
        public IndexedExchangeRate(IndexedExchangeRate indexedExchangeRate, IndexedExchangeRate? previousIndex = null)
        {
            _exchangeRate = indexedExchangeRate._exchangeRate;
            _previousIndexlinked = previousIndex;
        }

        /// <summary>
        /// The source Currency
        /// </summary>
        public Currency FromCurrency => _previousIndexlinked?.FromCurrency ?? _exchangeRate.FromCurrency;

        /// <summary>
        /// The destination Currency 
        /// </summary>
        public Currency ToCurrency => _exchangeRate.ToCurrency;

        /// <summary>
        /// The weight of the index.
        /// </summary>
        /// <remarks>It corresponds to the number of intermediate conversions.</remarks>
        public int Weight => (_previousIndexlinked?.Weight ?? 0) + 1;

        public async IAsyncEnumerable<decimal> Rates()
        {
            if (_previousIndexlinked is not null)
            {
                await foreach (var previousRate in _previousIndexlinked.Rates())
                {
                    yield return previousRate;
                }
            }
            yield return _exchangeRate.Rate;
        }

    }
}
