using System.Collections.Generic;

public static class Utils
{
    public static T RemoveAndGetElementFromList<T>(List<T> list, T searchEl)
    {
        T foundEl = list.Find((el) => ReferenceEquals(el, searchEl));
        list.Remove(foundEl);
        return foundEl;
    }
}