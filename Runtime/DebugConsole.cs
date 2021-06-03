using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole Instance;

    [SerializeField]
    private DebugPanel panel;

    public KeyCode key;
    public int fontSize = 20;

    private bool showConsole;
    private bool showHelp;
    private bool showInfo;

    private string input;

    private Vector2 scroll;

    private List<string> commandIdList;
    private List<string> commandFoundList;
    private int commandSelected;

    public List<DebugCommandBase> commandList;

    public void AddCommand(DebugCommandBase command)
    {
        commandList.Add(command);
        commandIdList.Add(command.GetCommandId());
    }

    public bool IsConsoleShowing()
    {
        return showConsole;
    }

    private void OnToggleDebug()
    {
        showConsole = !showConsole;
    }

    private void OnReturn()
    {
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        {
            if (input.Contains(commandList[i].GetCommandId()))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null)
                {
                    if (properties.Length > 1)
                    {
                        if (int.TryParse(properties[1], out _))
                            (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                    }
                }
                else if (commandList[i] as DebugCommand<double> != null)
                {
                    if (properties.Length > 1)
                    {
                        if (double.TryParse(properties[1], out _))
                            (commandList[i] as DebugCommand<double>).Invoke(double.Parse(properties[1]));
                    }
                }
                else if (commandList[i] as DebugCommand<float> != null)
                {
                    if (properties.Length > 1)
                    {
                        if (float.TryParse(properties[1], out _))
                            (commandList[i] as DebugCommand<float>).Invoke(float.Parse(properties[1]));
                    }
                }
                else if (commandList[i] as DebugCommand<bool> != null)
                {
                    if (properties.Length > 1)
                    {
                        if (bool.TryParse(properties[1], out _))
                            (commandList[i] as DebugCommand<bool>).Invoke(bool.Parse(properties[1]));
                    }
                }
                else if (commandList[i] as DebugCommand<int, int> != null)
                {
                    if (properties.Length > 2)
                    {
                        if (int.TryParse(properties[1], out _) && int.TryParse(properties[2], out _))
                            (commandList[i] as DebugCommand<int, int>).Invoke(int.Parse(properties[1]), int.Parse(properties[2]));
                    }
                }
                else if (commandList[i] as DebugCommand<double, double> != null)
                {
                    if (properties.Length > 2)
                    {
                        if (double.TryParse(properties[1], out _) && double.TryParse(properties[2], out _))
                            (commandList[i] as DebugCommand<double, double>).Invoke(double.Parse(properties[1]), double.Parse(properties[2]));
                    }
                }
                else if (commandList[i] as DebugCommand<float, float> != null)
                {
                    if (properties.Length > 2)
                    {
                        if (float.TryParse(properties[1], out _) && float.TryParse(properties[2], out _))
                            (commandList[i] as DebugCommand<float, float>).Invoke(float.Parse(properties[1]), float.Parse(properties[2]));
                    }
                }
            }
        }
    }

    private void ScrollBox(float y, bool help, GUIStyle style)
    {
        Rect viweport = new Rect(0, 0, Screen.width - (fontSize + 10), (fontSize + 10) * (help ? commandList.Count : panel.infoList.Count));

        scroll = GUI.BeginScrollView(new Rect(0, y, Screen.width, Screen.height / 6), scroll, viweport);

        for (int i = 0; i < ((help) ? commandList.Count : panel.infoList.Count); i++)
        {
            string label = help ? $"{commandList[i].GetCommandFormat()} - {commandList[i].GetCommandDescription()}" : $"{panel.infoList[i].GetInfoId()} : {panel.infoList[i].GetInfoDescription()}";

            Rect labelRect = new Rect(10, 5 + (fontSize + 10) * i, viweport.width - Screen.height / 6, fontSize + 10);
            GUI.Label(labelRect, label, style);
        }
    }

    private void Init()
    {
        DebugCommand HELP = new DebugCommand("help", "Shows a list of all available commands.", "help", () =>
        {
            showHelp = true;
            showInfo = false;
            scroll = Vector2.zero;
        });

        DebugCommand INFO = new DebugCommand("info", "Shows the description of all the information on the panel.", "info", () =>
        {
            showHelp = false;
            showInfo = true;
            scroll = Vector2.zero;
        });

        DebugCommand<int> CHANGE_SCENE = new DebugCommand<int>("change_scene", "Changes the current scene.", "change_scene<int>", (x) =>
        {
            if (x >= 0 && x < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(x);
        });

        commandList = new List<DebugCommandBase>
        {
            HELP,
            INFO,
            CHANGE_SCENE
        };

        commandIdList = new List<string>();
        for (int i = 0; i < commandList.Count; i++)
        {
            commandIdList.Add(commandList[i].GetCommandId());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Instance.Init();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
            OnToggleDebug();
    }

    private void OnGUI()
    {
        if (!showConsole) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 1, 1);
        style.fontSize = fontSize;

        Event e = Event.current;
        if (e.keyCode == KeyCode.Return) OnReturn();

        float y = 0;

        if (showHelp || showInfo)
        {
            GUI.Box(new Rect(0, y, Screen.width, Screen.height / 6), "");

            ScrollBox(y, showHelp, style);

            GUI.EndScrollView();
            y += Screen.height / 6;
        }

        GUI.Box(new Rect(0, y, Screen.width, fontSize + 10), "");
        GUI.backgroundColor = new Color(0, 0, 0);

        int length = 0;
        string oldString = input;
        if (oldString != null) length = oldString.Length;
        input = GUI.TextField(new Rect(10, y + 5, Screen.width - (fontSize + 10), fontSize + 10), input, style);
        y += fontSize + 10;

        if (!string.IsNullOrEmpty(input))
        {
            if (input.Length != length)
            {
                commandSelected = 0;
                commandFoundList = commandIdList.FindAll(w => w.StartsWith(input));
            }

            if (commandFoundList != null && commandFoundList.Count > 0)
            {
                GUI.Box(new Rect(0, y, Screen.width, commandFoundList.Count * (fontSize + 10)), "");

                for (int i = 0; i < commandFoundList.Count; i++)
                {
                    string label = commandFoundList[i];
                    Rect labelRect = new Rect(10, y + (fontSize + 10) * i, Screen.width - (fontSize + 10), fontSize + 10);
                    GUI.Label(labelRect, label, style);
                }

                GUI.Box(new Rect(0, y + (fontSize + 10) * commandSelected, Screen.width, fontSize + 10), "");

                if (e.keyCode == KeyCode.Tab)
                    input = commandFoundList[commandSelected];

                if (e.type == EventType.Used && e.keyCode == KeyCode.DownArrow)
                    commandSelected = (commandSelected + 1) < commandFoundList.Count ? commandSelected + 1 : commandFoundList.Count - 1;
                else if (e.type == EventType.Used && e.keyCode == KeyCode.UpArrow)
                    commandSelected = (commandSelected - 1) > 0 ? commandSelected - 1 : 0;

                y += commandFoundList.Count * (fontSize + 10);
            }
        }
    }
}
