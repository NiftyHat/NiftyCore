namespace NiftyFramework.Core
{
    public struct Pair<TType>
    {
        public TType Left { get; }
        public TType Right { get; }

        public Pair(TType left, TType right)
        {
            Left = left;
            Right = right;
        }
    }
}