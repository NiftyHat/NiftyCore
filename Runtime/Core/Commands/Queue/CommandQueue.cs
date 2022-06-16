using System;
using System.Collections.Generic;

namespace NiftyFramework.Core.Commands
{
    public class CommandQueue<TCommand> where TCommand : ICommand
    {
        public delegate void DelegateCommandResponse(TCommand command);
        
        public struct ProcessResponse
        {
            public int Processed;
            public int Skipped;
            public int Total;

            public int Remaining => Total - Processed;
        }
        
        protected readonly List<TCommand> _queue = new List<TCommand>();

        public List<TCommand> Items => _queue;
        public bool IsEmpty => _queue.Count > 0;

        public bool TryGetLast(out ICommand command)
        {
            if (_queue != null && _queue.Count > 0)
            {
                command = _queue[_queue.Count - 1];
                return true;
            }
            command = default(ICommand);
            return false;
        }

        public ProcessResponse Process(Action<TCommand> onProcessed = null, Action<TCommand> onInvalidated = null)
        {
            ProcessResponse response = new ProcessResponse
            {
                Total = _queue.Count
            };
            foreach (var item in _queue)
            {
                if (item.Validate())
                {
                    item.Execute();
                    onProcessed?.Invoke(item);
                    continue;
                }
                onInvalidated?.Invoke(item);
            }
            Clear();
            return response;
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public bool Add(IEnumerable<TCommand> commandList, DelegateCommandResponse onInvalid = null)
        {
            bool isAllValid = true;
            foreach (var item in commandList)
            {
                isAllValid = Add(item, onInvalid) && isAllValid;
            }
            return isAllValid;
        }

        public bool Add(TCommand command, DelegateCommandResponse onInvalid = null)
        {
            if (command.Validate())
            {
                _queue.Add(command);
                return true;
            }
            
            if (onInvalid != null)
            {
                onInvalid.Invoke(command);
            }
            else
            {
                throw new CommandException($"Unhandled instance of Invalid {nameof(command)} added to queue {this}");
            }
            return false;
        }

        public bool Create<TUpdate>(Func<TUpdate> factoryMethod) where TUpdate : TCommand, new()
        {
            TUpdate item = factoryMethod.Invoke();
            if (item == null)
            {
                throw new NullReferenceException($"{nameof(CommandQueue<TCommand>)}.{nameof(Create)}() param {nameof(factoryMethod)} returned null instance");
            }
            return Add(item);
        }
    }
}