namespace NiftyFramework.Core
{
    public delegate void Updated();
    public interface IUpdateable
    {
        event Updated OnUpdated;

        void Update(float delta = 1);
    }
}