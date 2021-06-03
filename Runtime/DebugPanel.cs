using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugPanel : MonoBehaviour
{
    public static DebugPanel Instance;

    [SerializeField]
    private DebugConsole console;

    public int fontSize = 20;

    private Vector2 scroll;

    public List<DebugInfoBase> infoList;

    private float deltaTime = 0;
    private float fps = 0;

    public void AddInfo(DebugInfoBase info)
    {
        infoList.Add(info);
    }

    public int GetSceneNumber()
    {
        return SceneManager.sceneCountInBuildSettings;
    }

    public int GetCurrentScene()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    private void Init()
    {
        DebugInfo<float> FPS = new DebugInfo<float>("FPS", "Shows the frames per second.", () => { return fps; });

        DebugInfo<int> SCENE_NUMBER = new DebugInfo<int>("Number of Scenes", "Amount of scenes in the build.", () => { return GetSceneNumber(); });

        DebugInfo<int> CURRENT_SCENE = new DebugInfo<int>("Current Scene", "Index in the build of the current scene.", () => { return GetCurrentScene(); });

        infoList = new List<DebugInfoBase>()
        {
            FPS,
            SCENE_NUMBER,
            CURRENT_SCENE
        };
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
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;
    }

    private void OnGUI()
    {
        if (!console || !console.IsConsoleShowing()) return;

        GUIStyle style = new GUIStyle();
        style.normal.textColor = new Color(1, 1, 1);
        style.fontSize = fontSize;

        GUI.Box(new Rect(Screen.width * 2 / 3, Screen.height * 2 / 3, Screen.width / 3, Screen.height / 3), "");

        Rect viweport = new Rect(Screen.width * 2 / 3, Screen.height * 2 / 3, Screen.width / 3 - (fontSize + 10), (fontSize + 10) * infoList.Count);

        scroll = GUI.BeginScrollView(new Rect(Screen.width * 2 / 3, Screen.height * 2 / 3, Screen.width / 3, Screen.height / 3), scroll, viweport);

        for (int i = 0; i < infoList.Count; i++)
        {
            string label = $"{infoList[i].GetInfoId()}";

            if (infoList[i] as DebugInfo<int> != null)
            {
                (infoList[i] as DebugInfo<int>).GetInfo(out int help);
                label = $"{label} : {help}";
            }
            else if (infoList[i] as DebugInfo<double> != null)
            {
                (infoList[i] as DebugInfo<double>).GetInfo(out double help);
                label = $"{label} : {help}";
            }
            else if (infoList[i] as DebugInfo<float> != null)
            {
                (infoList[i] as DebugInfo<float>).GetInfo(out float help);
                label = $"{label} : {help}";
            }
            else if (infoList[i] as DebugInfo<bool> != null)
            {
                (infoList[i] as DebugInfo<bool>).GetInfo(out bool help);
                label = $"{label} : {help}";
            }
            else if (infoList[i] as DebugInfo<System.ValueType> != null)
            {
                (infoList[i] as DebugInfo<System.ValueType>).GetInfo(out System.ValueType help);
                label = $"{label} : {help}";
            }
            else if (infoList[i] as DebugInfo<int, int> != null)
            {
                (infoList[i] as DebugInfo<int, int>).GetInfo(out int help, out int help2);
                label = $"{label} : {help} , {help2}";
            }
            else if (infoList[i] as DebugInfo<double, double> != null)
            {
                (infoList[i] as DebugInfo<double, double>).GetInfo(out double help, out double help2);
                label = $"{label} : {help} , {help2}";
            }
            else if (infoList[i] as DebugInfo<float, float> != null)
            {
                (infoList[i] as DebugInfo<float, float>).GetInfo(out float help, out float help2);
                label = $"{label} : {help} , {help2}";
            }
            else if (infoList[i] as DebugInfo<bool, bool> != null)
            {
                (infoList[i] as DebugInfo<bool, bool>).GetInfo(out bool help, out bool help2);
                label = $"{label} : {help} , {help2}";
            }
            else if (infoList[i] as DebugInfo<System.ValueType, System.ValueType> != null)
            {
                (infoList[i] as DebugInfo<System.ValueType, System.ValueType>).GetInfo(out System.ValueType help, out System.ValueType help2);
                label = $"{label} : {help} , {help2}";
            }

            Rect labelRect = new Rect(Screen.width * 2 / 3 + 10, Screen.height * 2 / 3 + (5 + (fontSize + 10) * i), Screen.width / 3, fontSize + 10);
            GUI.Label(labelRect, label, style);
        }

        GUI.EndScrollView();
    }
}
