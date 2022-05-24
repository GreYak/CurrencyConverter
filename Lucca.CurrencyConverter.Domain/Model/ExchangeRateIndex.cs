using Lucca.CurrencyConverter.Domain.Exceptions;
using Lucca.CurrencyConverter.Domain.Tools;

namespace Lucca.CurrencyConverter.Domain.Model
{
    /// <summary>
    /// Represent the indexation of all the exchange rates.
    /// </summary>
    internal class ExchangeRateIndex
    {
        private readonly Dictionary<Currency, IndexedExchangesRateCollection> _mainIndex;
        private readonly DictionaryOfList<Currency, IndexedExchangeRate> _reverseIndex;

        /// <summary>
        /// Create a new instance of <see cref="ExchangeRateIndex"/>.
        /// </summary>
        public ExchangeRateIndex()
        {
            _mainIndex = new Dictionary<Currency, IndexedExchangesRateCollection>();
            _reverseIndex = new DictionaryOfList<Currency, IndexedExchangeRate>();
        }

        /// <summary>
        /// Reinitialize the index.
        /// </summary>
        public void Clear()
        {
            _mainIndex.Clear();
            _reverseIndex.Clear();
        }

        /// <summary>
        /// Fetch the specified exchange rate in the index.
        /// </summary>
        /// <param name="fromCurrency">Currency to change.</param>
        /// <param name="toCurrency">Currency in which changing <paramref name="fromCurrency"/></param>
        /// <returns><see cref="ExchangeRate"/></returns>
        /// <exception cref="IndexNotFoundException"></exception>
        public IndexedExchangeRate GetExchangeRate(Currency fromCurrency, Currency toCurrency)
        {
            if (_mainIndex.ContainsKey(fromCurrency) && _mainIndex[fromCurrency].TryToGetValue(toCurrency, out IndexedExchangeRate? indexedExchangeRate))
                return indexedExchangeRate!;

            throw new IndexNotFoundException($"ExchangeRate {fromCurrency}/{toCurrency} doesn't exist in the registry.");
        }

        /// <summary>
        /// Register an exchange rate into the current index.
        /// </summary>
        /// <param name="exchangeRate"><see cref="ExchangeRate"/></param>
        public void AddExchangeRate(ExchangeRate exchangeRate)
        {
            var indexedExchangeRate = new IndexedExchangeRate(exchangeRate);
            AddIndexedExchangeRate(indexedExchangeRate);
            ExtendExchangeRateByLinkingAfter(exchangeRate);
            ExtendExchangeRateByLinkingBefore(indexedExchangeRate);
        }

        private void ExtendExchangeRateByLinkingAfter(ExchangeRate exchangeRateExtension)
        {
            if (_reverseIndex.TryGetValue(exchangeRateExtension.FromCurrency, out IReadOnlyList<IndexedExchangeRate>? exchangeRatesToExtendCollection))
            {
                var indexesToAdd = new List<IndexedExchangeRate>();
                foreach (IndexedExchangeRate indexedExchangeRateToExtend in exchangeRatesToExtendCollection!)
                {
                    if (exchangeRateExtension.ToCurrency != indexedExchangeRateToExtend.FromCurrency)         
                    {
                        var linkedExchangeRate = new IndexedExchangeRate(exchangeRateExtension, indexedExchangeRateToExtend);
                        indexesToAdd.Add(linkedExchangeRate);
                    }
                }
                indexesToAdd.ForEach(i => AddIndexedExchangeRate(i));
            }
        }

        private void ExtendExchangeRateByLinkingBefore(IndexedExchangeRate indexedExchangeRateToExtend)
        {
            if (_mainIndex.TryGetValue(indexedExchangeRateToExtend.ToCurrency, out IndexedExchangesRateCollection? exchangeRateExtensionsCollection))
            {
                var indexesToAdd = new List<IndexedExchangeRate>();
                foreach (IndexedExchangeRate exchangeRateExtension in exchangeRateExtensionsCollection)
                {
                    if (exchangeRateExtension.ToCurrency != indexedExchangeRateToExtend.FromCurrency)            
                    {
                        var linkedExchangeRate = new IndexedExchangeRate(exchangeRateExtension, indexedExchangeRateToExtend);
                        indexesToAdd.Add(linkedExchangeRate);
                    }
                }
                indexesToAdd.ForEach(i => AddIndexedExchangeRate(i));
            }
        }

        private void AddIndexedExchangeRate(IndexedExchangeRate indexedExchangeRate)
        {
            // Main index
            Currency key = indexedExchangeRate.FromCurrency;
            if (!_mainIndex.ContainsKey(key))
            {
                _mainIndex[key] = new IndexedExchangesRateCollection();
            }
            _mainIndex[key].Add(indexedExchangeRate);

            // Reverse Index
            _reverseIndex.Add(indexedExchangeRate.ToCurrency, indexedExchangeRate);
        }
    }
}
