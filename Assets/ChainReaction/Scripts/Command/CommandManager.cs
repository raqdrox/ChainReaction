using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Athena.ChainReaction
{
    public class CommandManager
    {
        Stack<ICommand> _commandStack;
        Stack<ICommand> _redoStack;

        Queue<ICommand> _commandQueue;

        public bool CanUndo => !(_commandStack.Count == 0); 
        public bool CanRedo => !(_redoStack.Count == 0); 
        
        public CommandManager()
        {
            _commandStack = new Stack<ICommand>();
            _redoStack = new Stack<ICommand>();
            _commandQueue = new Queue<ICommand>();
        }

        public void RunCommandAndSave(ICommand newCommand)
        {
            newCommand.Execute();
            _commandStack.Push(newCommand);

            _redoStack.Clear();
        }
        public void RunCommand(ICommand newCommand)
        {
            newCommand.Execute();
        }
        public void AddCommandToQueue(ICommand newCommand)
        {
            _commandQueue.Enqueue(newCommand);
        }
        public void RunQueue()
        {
            while (_commandQueue.Count>0)
            {
                var command = _commandQueue.Dequeue();
                command.Execute();
            }
        }
        public void UndoCommand()
        {
            if (_commandStack.Count == 0)
                return;
            var command = _commandStack.Pop();
            _redoStack.Push(command);
            command.Undo();
        }

        public void RedoCommand()
        {
            if (_redoStack.Count == 0)
                return;
            var command = _redoStack.Pop();
            _commandStack.Push(command);
            command.Execute();
        }
        public ICommand GetLastCommand()
        {
            return _commandStack.Peek();
        }

    }
}
