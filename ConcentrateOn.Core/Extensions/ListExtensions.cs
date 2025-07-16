namespace ConcentrateOn.Core.Extensions;

public static class ListExtensions
{
    public static List<T> Subtract<T>(this List<T> source, List<T> target)
    {
        var subtractedList = new List<T>(source);
        foreach (var t in target)
            subtractedList.Remove(t);

        return subtractedList;
    }
}
