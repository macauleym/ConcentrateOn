namespace ConcentrateOn.Core.Extensions;

public static class TypeExtensions
{
    public static U PipeTo<T, U>(this T source, Func<T, U> next) =>
        next(source);

    public static void PipeTo<T>(this T source, Action<T> final) =>
        final(source);
}
