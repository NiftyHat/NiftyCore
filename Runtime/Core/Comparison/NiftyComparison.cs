using System;

namespace NiftyFramework.Core
{
    public static class NiftyComparison
    {
        public static string[] SerializableValues = { "==", "!=", ">", ">=", "<", "<=" };
    }
    
    public static class NiftyComparison<TValue> where TValue : IComparable<TValue>
    {
        public static Equal<TValue> Equal = new Equal<TValue>();
        public static NotEqual<TValue> NotEqual = new NotEqual<TValue>();
        public static Greater<TValue> Greater = new Greater<TValue>();
        public static Less<TValue> Less = new Less<TValue>();
        public static GreaterOrEqual<TValue> GreaterOrEqual = new GreaterOrEqual<TValue>();
        public static LessOrEqual<TValue> LessOrEqual = new LessOrEqual<TValue>();
        
        public static IComparison<TValue> StringToComparison(string value)
        {
            switch (value)
            {
                case "==":
                    return Equal;
                case "!=":
                    return NotEqual;
                case ">":
                    return Greater;;
                case ">=":
                    return GreaterOrEqual;
                case "<":
                    return Less;
                case "<=":
                    return LessOrEqual;
                default:
                    return null;
            }
        }
    }
    
    public interface IComparison<TValue> where TValue : IComparable<TValue>
    {
        bool Compare(TValue left, TValue right);
        public string Symbol { get; }
    }
    
    public struct Equal<TValue> : IComparison<TValue> where TValue :  IComparable<TValue>
    {
        public string Symbol => "==";

        public bool Compare(TValue left, TValue right)
        {
            return left.CompareTo(right) == 0;
        }
    }
    
    public struct NotEqual<TValue> : IComparison<TValue> where TValue :  IComparable<TValue>
    {
        public string Symbol => "!=";
        public bool Compare(TValue left, TValue right)
        {
            return left.CompareTo(right) != 0;
        }
    }

    public struct Greater<TValue> : IComparison<TValue> where TValue : IComparable<TValue>
    {
        public string Symbol => ">";
        public bool Compare(TValue left, TValue right)
        {
            return left.CompareTo(right) >= 1;
        }
    }

    public struct Less<TValue> : IComparison<TValue> where TValue : IComparable<TValue>
    {
        public string Symbol => "<";
        public bool Compare(TValue left, TValue right)
        {
            return left.CompareTo(right) <= -1;
        }
    }
    
    public struct LessOrEqual<TValue> : IComparison<TValue> where TValue : IComparable<TValue>
    {
        public string Symbol => "<=";
        public bool Compare(TValue left, TValue right)
        {
            return left.CompareTo(right) == 0 || left.CompareTo(right) <= -1;
        }
    }

    public struct GreaterOrEqual<TValue> : IComparison<TValue> where TValue : IComparable<TValue>
    {
        public string Symbol => ">=";
        public bool Compare(TValue left, TValue right)
        {
            return left.CompareTo(right) == 0 || left.CompareTo(right) <= 1;
        }
    }
    
}