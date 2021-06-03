using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugCommandBase
{
    private string commandId;
    private string commandDescription;
    private string commandFormat;

    public DebugCommandBase(string id, string description, string format)
    {
        commandId = id;
        commandDescription = description;
        commandFormat = format;
    }

    public string GetCommandId() { return commandId; }
    public string GetCommandDescription() { return commandDescription; }
    public string GetCommandFormat() { return commandFormat; }
}

public class DebugCommand : DebugCommandBase
{
    private Action command;

    public DebugCommand(string id, string description, string format, Action command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke()
    {
        command.Invoke();
    }
}

public class DebugCommand<T> : DebugCommandBase
{
    private Action<T> command;

    public DebugCommand(string id, string description, string format, Action<T> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T value)
    {
        command.Invoke(value);
    }
}

public class DebugCommand<T, N> : DebugCommandBase
{
    private Action<T, N> command;

    public DebugCommand(string id, string description, string format, Action<T, N> command) : base(id, description, format)
    {
        this.command = command;
    }

    public void Invoke(T value1, N value2)
    {
        command.Invoke(value1, value2);
    }
}
