namespace NiftyFramework.Core.Commands
{
    public interface ICommand
    {
        bool Validate();
        void Execute();
        
    }
}