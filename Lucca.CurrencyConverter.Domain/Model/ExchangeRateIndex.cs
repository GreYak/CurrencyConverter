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
        public ExchangeRate GetExchangeRate(Currency fromCurrency, Currency toCurrency)
        {
            if (_mainIndex.ContainsKey(fromCurrency) && _mainIndex[fromCurrency].TryToGetValue(toCurrency, out IndexedExchangeRate? indexedExchangeRate))
                return indexedExchangeRate!.ExchangeRate;

            throw new IndexNotFoundException($"ExchangeRate {fromCurrency}/{toCurrency} doesn't exist in the registry.");
        }

        /// <summary>
        /// Register an exchange rate into the current index.
        /// </summary>
        /// <param name="exchangeRate"><see cref="ExchangeRate"/></param>
        public void AddExchangeRate(ExchangeRate exchangeRate)
        {
            var indexedExchangeRate = new IndexedExchangeRate(exchangeRate);
            AddExchangeRate(indexedExchangeRate);
            ExtendExchangeRateByLinkingAfter(exchangeRate);
            ExtendExchangeRateByLinkingBefore(exchangeRate);
        }

        private void ExtendExchangeRateByLinkingAfter(ExchangeRate exchangeRateToExtend)
        {
            if (_mainIndex.TryGetValue(exchangeRateToExtend.ToCurrency, out IndexedExchangesRateCollection? extensionRatesCollection))
            {
                foreach (IndexedExchangeRate extensionRate in extensionRatesCollection)
                {
                    if (extensionRate.ExchangeRate.ToCurrency != exchangeRateToExtend.FromCurrency)
                    {
                        var newRate = extensionRate.ExchangeRate.Rate * exchangeRateToExtend.Rate;
                        var linkedExchangeRate = new IndexedExchangeRate(exchangeRateToExtend.FromCurrency,
                                                                            extensionRate.ExchangeRate.ToCurrency, 
                                                                            newRate, 
                                                                            extensionRate);
                        AddExchangeRate(linkedExchangeRate);
                    }
                }
            }
        }

        private void ExtendExchangeRateByLinkingBefore(ExchangeRate exchangeRateExtension)
        {
            if (_reverseIndex.TryGetValue(exchangeRateExtension.FromCurrency, out IReadOnlyList<IndexedExchangeRate>? exchangeRatesToExtendCollection))
            {
                foreach (IndexedExchangeRate exchangeRateToExtend in exchangeRatesToExtendCollection!)
                {
                    if (exchangeRateToExtend.ExchangeRate.FromCurrency != exchangeRateExtension.ToCurrency)
                    {
                        var newRate = exchangeRateToExtend.ExchangeRate.Rate * exchangeRateExtension.Rate;
                        var linkedExchangeRate = new IndexedExchangeRate(exchangeRateToExtend.ExchangeRate.FromCurrency,
                                                                            exchangeRateExtension.ToCurrency,
                                                                            newRate,
                                                                            exchangeRateToExtend);
                        AddExchangeRate(linkedExchangeRate);
                    }
                }
            }
        }

        private void AddExchangeRate(IndexedExchangeRate indexedExchangeRate)
        {
            // Main index
            Currency key = indexedExchangeRate.ExchangeRate.FromCurrency;
            if (!_mainIndex.ContainsKey(key))
            {
                _mainIndex[key] = new IndexedExchangesRateCollection();
            }
            _mainIndex[key].Add(indexedExchangeRate);

            // Reverse Index
            _reverseIndex.Add(indexedExchangeRate.ExchangeRate.ToCurrency, indexedExchangeRate);
        }
    }
}
