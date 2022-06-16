using System;
using NiftyFramework.Services;

namespace NiftyFramework.Core.SubSystems
{
    [NiftyService]
    public interface ILoggerNiftyService
    {
        void Log(string tag, object message);
        void LogWarning(string tag, object message);
        void LogError(string tag, object message);
        void LogException(Exception exception);
    }
}