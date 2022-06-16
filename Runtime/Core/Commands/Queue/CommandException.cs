using System;

namespace NiftyFramework.Core.Commands
{
    public class CommandException : Exception
    {
        public CommandException() : base()
        {
            
        }
        
        public CommandException(string message) : base(message)
        {
            
        }
        
        public CommandException(string message, Exception baseException) : base(message, baseException)
        {
            
        }
    }
}