#define TRACE

using System;
using System.Diagnostics;
using NiftyFramework.Core.Services;

namespace NiftyFramework.Core.SubSystems
{
    public class TraceLoggingNiftyService : NiftyService
    {
        public void Dispose(Action onComplete)
        {
            onComplete();
        }

        public void Log(string tag, object message)
        {
            Trace.Write(message, tag);
        }

        public void LogWarning(string tag, object message)
        {
            Trace.TraceWarning(tag, message);
        }

        public void LogError(string tag, object message)
        {
            Trace.TraceError(tag, message);
        }

        public void LogException(Exception exception)
        {
            Trace.TraceError("Exception", exception);
        }

        public override void Init(NiftyService.OnReady onReady)
        {
            Trace.Write("NiftyFramework - TraceLoggingSubSystem Init");
            onReady();
        }
    }
}