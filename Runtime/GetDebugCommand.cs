using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GetDebugCommand : MonoBehaviour
{
    public string commandId;
    public string commandDescription;
    public string commandFormat;

    [HideInInspector]
    public int parameterNum;

    [HideInInspector]
    public MonoBehaviour script;
    [HideInInspector]
    public string functionName, type;

    private void Start()
    {
        if (!string.IsNullOrEmpty(functionName))
        {
            if (parameterNum == 0)
            {
                DebugCommand COMMAND = new DebugCommand(commandId, commandDescription, commandFormat, () =>
                {
                    script.GetType().GetMethod(functionName)?.Invoke(script, null);
                });
                DebugConsole.Instance.AddCommand(COMMAND);
            }
            else if (parameterNum == 1)
            {
                Type aux = Type.GetType(type);
                if (aux == typeof(int))
                {
                    DebugCommand<int> COMMAND = new DebugCommand<int>(commandId, commandDescription, commandFormat, (x) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[1] { x });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
                else if (aux == typeof(double))
                {
                    DebugCommand<double> COMMAND = new DebugCommand<double>(commandId, commandDescription, commandFormat, (x) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[1] { x });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
                else if (aux == typeof(float))
                {
                    DebugCommand<float> COMMAND = new DebugCommand<float>(commandId, commandDescription, commandFormat, (x) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[1] { x });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
                else if (aux == typeof(bool))
                {
                    DebugCommand<bool> COMMAND = new DebugCommand<bool>(commandId, commandDescription, commandFormat, (x) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[1] { x });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
            }
            else
            {
                Type aux = Type.GetType(type);
                if (aux == typeof(int))
                {
                    DebugCommand<int, int> COMMAND = new DebugCommand<int, int>(commandId, commandDescription, commandFormat, (x, y) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[2] { x, y });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
                else if (aux == typeof(double))
                {
                    DebugCommand<double, double> COMMAND = new DebugCommand<double, double>(commandId, commandDescription, commandFormat, (x, y) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[2] { x, y });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
                else if (aux == typeof(float))
                {
                    DebugCommand<float, float> COMMAND = new DebugCommand<float, float>(commandId, commandDescription, commandFormat, (x, y) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[2] { x, y });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
                else if (aux == typeof(bool))
                {
                    DebugCommand<bool, bool> COMMAND = new DebugCommand<bool, bool>(commandId, commandDescription, commandFormat, (x, y) =>
                    {
                        script.GetType().GetMethod(functionName)?.Invoke(script, new object[2] { x, y });
                    });
                    DebugConsole.Instance.AddCommand(COMMAND);
                }
            }
        }
    }
}
