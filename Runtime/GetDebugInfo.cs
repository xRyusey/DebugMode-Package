using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDebugInfo : MonoBehaviour
{
    public string infoId;
    public string infoDescription;

    [HideInInspector]
    public int getterNum;
    [HideInInspector]
    public MonoBehaviour script1, script2;
    [HideInInspector]
    public string fieldName1, fieldName2;
    [HideInInspector]
    public string functionName1, functionName2;

    private System.ValueType value1, value2;

    void Start()
    {
        if ((!string.IsNullOrEmpty(fieldName1) || !string.IsNullOrEmpty(functionName1)))
        {
            if (getterNum == 1)
            {
                DebugInfo<System.ValueType> INFO = new DebugInfo<System.ValueType>(infoId, infoDescription, () =>
                {
                    if (!string.IsNullOrEmpty(fieldName1))
                    {
                        value1 = (System.ValueType)script1.GetType().GetField(fieldName1)?.GetValue(script1);
                    }
                    else if (!string.IsNullOrEmpty(functionName1))
                    {
                        value1 = (System.ValueType)script1.GetType().GetMethod(functionName1)?.Invoke(script1, null);
                    }
                    return value1;
                });
                DebugPanel.Instance.AddInfo(INFO);
            }
            else
            {
                if ((!string.IsNullOrEmpty(fieldName2) || !string.IsNullOrEmpty(functionName2)))
                {
                    DebugInfo<System.ValueType, System.ValueType> INFO = new DebugInfo<System.ValueType, System.ValueType>(infoId, infoDescription, () =>
                    {
                        if (!string.IsNullOrEmpty(fieldName1))
                        {
                            value1 = (System.ValueType)script1.GetType().GetField(fieldName1)?.GetValue(script1);
                        }
                        else if (!string.IsNullOrEmpty(functionName1))
                        {
                            value1 = (System.ValueType)script1.GetType().GetMethod(functionName1)?.Invoke(script1, null);
                        }
                        return value1;
                    }, () =>
                    {
                        if (!string.IsNullOrEmpty(fieldName2))
                        {
                            value2 = (System.ValueType)script1.GetType().GetField(fieldName2)?.GetValue(script1);
                        }
                        else if (!string.IsNullOrEmpty(functionName2))
                        {
                            value2 = (System.ValueType)script1.GetType().GetMethod(functionName2)?.Invoke(script1, null);
                        }
                        return value2;
                    });

                    DebugPanel.Instance.AddInfo(INFO);
                }
            }
        }
    }
}
