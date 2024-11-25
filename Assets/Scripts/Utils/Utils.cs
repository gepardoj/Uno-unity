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

    public static T RandomEnum<T>() where T : Enum
    {
        var length = Enum.GetNames(typeof(T)).Length;
        var value = UnityEngine.Random.Range(0, length);
        if (!Enum.IsDefined(typeof(T), value)) throw new Exception($"Invalid enum value {value}");
        return (T)Convert.ChangeType(value, typeof(int));
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    {
        return items.GroupBy(property).Select(x => x.First());
    }

    public static byte[][] Split(this byte[] array, byte separator) {
        var list = new List<byte[]>();
        var start = 0;
        for (var i = 0; i < array.Length; i++) {
            var value = array[i];
            if (value == separator) {
                list.Add(array.Skip(start).TakeWhile(_ => _ != separator).ToArray());
                start = i + 1;
            }
        }
        var tail = array.Skip(start).ToArray();
        if (tail.Length > 0) list.Add(tail);
        return list.ToArray();
    }
}