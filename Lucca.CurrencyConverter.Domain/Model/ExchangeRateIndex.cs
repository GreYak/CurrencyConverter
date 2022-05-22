using System.Collections;

namespace Lucca.CurrencyConverter.Domain.Model
{
    internal class DictionaryOfList<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, List<TValue>> _dic;
        public DictionaryOfList()
        {
            _dic = new Dictionary<TKey, List<TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, new List<TValue>());
            }
            _dic[key].Add(value);
        }

        public void Clear()
        {
            foreach (var list in _dic.Values)
            {
                list.Clear();
            }
            _dic.Clear();
        }

        internal bool TryGetValue(TKey key, out IReadOnlyList<TValue>? values)
        {
            var found = _dic.TryGetValue(key, out List<TValue>? listFound);
            values = listFound?.AsReadOnly();
            return found;
        }
    }

    internal record IndexedExchangeRate(ExchangeRate ExchangeRate)
    {
        private IndexedExchangeRate? _linked;

        public IndexedExchangeRate(Currency from, Currency to, decimal changeRate, IndexedExchangeRate linked):this (new ExchangeRate(from, to, changeRate))
        {
            _linked = linked;
        }

        public int Weight => (_linked?.Weight ?? 0) + 1;
    }

    internal class IdexedExchangeRateCollection : IEnumerable
    {
        public readonly Dictionary<Currency, IndexedExchangeRate> _exchangeRatesByToCurrency;

        public IdexedExchangeRateCollection()
        {
            _exchangeRatesByToCurrency = new Dictionary<Currency, IndexedExchangeRate>();
        }

        public void Add(IndexedExchangeRate indexedExchangeRate)
        {
            Currency key = indexedExchangeRate.ExchangeRate.ToCurrency;
            if (!_exchangeRatesByToCurrency.ContainsKey(key))
            {
                _exchangeRatesByToCurrency.Add(key, indexedExchangeRate);
            }
            else
            {
                if (_exchangeRatesByToCurrency[key].Weight > indexedExchangeRate.Weight)
                {
                    _exchangeRatesByToCurrency[key] = indexedExchangeRate;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _exchangeRatesByToCurrency.Values.GetEnumerator();
        }
    }

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
