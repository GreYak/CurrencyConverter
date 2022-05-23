namespace Lucca.CurrencyConverter.Domain.Tools
{
    /// <summary>
    /// Allow to manage a dictionary of list of <typeparamref name="TValue"/> entities, indexed by a <typeparamref name="TKey"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the index.</typeparam>
    /// <typeparam name="TValue">The type of elements in the list of values.</typeparam>
    internal class DictionaryOfList<TKey, TValue>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, List<TValue>> _dic;

        /// <summary>
        /// Initialize a new instance of <see cref="DictionaryOfList<TKey, TValue>"/>
        /// </summary>
        public DictionaryOfList()
        {
            _dic = new Dictionary<TKey, List<TValue>>();
        }

        /// <summary>
        /// Add a value item in the right indexed list.
        /// </summary>
        /// <param name="key">The key indexing the list.</param>
        /// <param name="value">The value to add.</param>
        public void Add(TKey key, TValue value)
        {
            if (!_dic.ContainsKey(key))
            {
                _dic.Add(key, new List<TValue>());
            }
            _dic[key].Add(value);
        }

        /// <summary>
        /// To remove all the stored items.
        /// </summary>
        public void Clear()
        {
            foreach (var list in _dic.Values)
            {
                list.Clear();
            }
            _dic.Clear();
        }

        /// <summary>
        /// Try to get the list indexed by the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="values">The list of indexed values if exist, else null.</param>
        /// <returns>True if a list is indexed by <paramref name="key"/>, else null.</returns>
        internal bool TryGetValue(TKey key, out IReadOnlyList<TValue>? values)
        {
            var found = _dic.TryGetValue(key, out List<TValue>? listFound);
            values = listFound?.AsReadOnly();
            return found;
        }
    }
}
