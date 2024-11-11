using System;
using System.Collections.Generic;
using System.Linq;

public static class Utils
{
    public static T RemoveAndGetElement<T>(List<T> list, T searchEl)
    {
        T foundEl = list.Find((el) => ReferenceEquals(el, searchEl));
        list.Remove(foundEl);
        return foundEl;
    }
    public static List<T> RemoveAndGetFirstNElements<T>(List<T> list, int amount)
    {
        var foundElements = list.GetRange(0, amount);
        list.RemoveRange(0, amount);
        return foundElements;
    }

    public static T RandomEnum<T>()
    {
        var length = Enum.GetNames(typeof(T)).Length;
        return (T)Convert.ChangeType(UnityEngine.Random.Range(0, length), typeof(T));
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    {
        return items.GroupBy(property).Select(x => x.First());
    }
}