namespace Lucca.CurrencyConverter.Domain.Tools
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
}
