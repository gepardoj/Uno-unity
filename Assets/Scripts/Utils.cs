using System.Collections.Generic;

public static class Utils
{
    public static T RemoveAndGetElement<T>(List<T> list, T searchEl)
    {
        T foundEl = list.Find((el) => ReferenceEquals(el, searchEl));
        list.Remove(foundEl);
        return foundEl;
    }
    public static List<T> RemoveAndGetFirstElements<T>(List<T> list, int amount)
    {
        var foundElements = list.GetRange(0, amount);
        list.RemoveRange(0, amount);
        return foundElements;
    }
}