namespace NiftyFramework.Core.Utils
{
    public static class StringExtension
    {
        public static bool HasValue(this string str) => str != null && str.Length > 0;
    }
}