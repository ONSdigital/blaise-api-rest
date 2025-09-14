namespace Blaise.Api.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class DictionaryExtensions
    {
        // will throw error if any key in dictionaryToAdd already exists in baseDictionary
        public static void AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> baseDictionary,
            IDictionary<TKey, TValue> dictionaryToAdd)
        {
            dictionaryToAdd.ForEach(x => baseDictionary.Add(x.Key, x.Value));
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}
