using Lucca.CurrencyConverter.Domain.Tools;
using System.Collections;

namespace Lucca.CurrencyConverter.Domain.Model
{
    internal class ExchangeRateIndex
    {
        private readonly Dictionary<Currency, IdexedExchangeRateCollection> _mainIndex;
        private readonly DictionaryOfList<Currency, IndexedExchangeRate> _reverseIndex;

        public ExchangeRateIndex()
        {
            _mainIndex = new Dictionary<Currency, IdexedExchangeRateCollection>();
            _reverseIndex = new DictionaryOfList<Currency, IndexedExchangeRate>();
        }

        public void Clear()
        {
            _mainIndex.Clear();
            _reverseIndex.Clear();
        }

        private void AddExchangeRate(IndexedExchangeRate rate)
        {
            // Main index
            Currency key = rate.ExchangeRate.FromCurrency;
            if (!_mainIndex.ContainsKey(key))
            {
                _mainIndex[key] = new IdexedExchangeRateCollection();
            }
            _mainIndex[key].Add(rate);

            // Reverse Index
            _reverseIndex.Add(rate.ExchangeRate.ToCurrency, rate);
        }

        internal ExchangeRate GetExchangeRate(Currency fromCurrency, Currency toCurrency)
        {
            if (_mainIndex.ContainsKey(fromCurrency) && _mainIndex[fromCurrency].TryToGetValue(toCurrency, out IndexedExchangeRate? indexedExchangeRate))
                return indexedExchangeRate!.ExchangeRate;

            throw new ApplicationException($"ExchangeRate {fromCurrency}/{toCurrency} doesn't exist in the registry.");
        }

        public void AddExchangeRate(ExchangeRate rate)
        {
            var indexedExchangeRate = new IndexedExchangeRate(rate);
            AddExchangeRate(indexedExchangeRate);
            ExtendExchangeRateByLinkingAfter(rate);
            ExtendExchangeRateByLinkingBefore(rate);
        }

        private void ExtendExchangeRateByLinkingAfter(ExchangeRate exchangeRateToExtend)
        {
            if (_mainIndex.TryGetValue(exchangeRateToExtend.ToCurrency, out IdexedExchangeRateCollection? extensionRatesCollection))
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
    }
}
