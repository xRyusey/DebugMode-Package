using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DebugInfoBase
{
    private string infoId;
    private string infoDescription;

    public DebugInfoBase(string id, string description)
    {
        infoId = id;
        infoDescription = description;
    }

    public string GetInfoId() { return infoId; }
    public string GetInfoDescription() { return infoDescription; }
}

public class DebugInfo<T> : DebugInfoBase
{
    private Func<T> getter;

    public DebugInfo(string id, string description, Func<T> getter) : base(id, description)
    {
        this.getter = getter;
    }

    public void GetInfo(out T value)
    {
        value = getter.Invoke();
    }
}

public class DebugInfo<T, N> : DebugInfoBase
{
    private Func<T> getter1;
    private Func<N> getter2;

    public DebugInfo(string id, string description, Func<T> getter1, Func<N> getter2) : base(id, description)
    {
        this.getter1 = getter1;
        this.getter2 = getter2;
    }

    public void GetInfo(out T value1, out N value2)
    {
        value1 = getter1.Invoke();
        value2 = getter2.Invoke();
    }
}
