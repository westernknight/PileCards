#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：ExtensionMethods
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：7/8/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{

    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }
    public static GameObject SelfCopy(this GameObject trans,string name)
    {

        GameObject go = GameObject.Instantiate(trans) as GameObject;
        go.name = name;
        go.transform.parent = trans.transform.parent;
        go.transform.localPosition = trans.transform.localPosition;
        go.transform.localRotation = trans.transform.localRotation;
        go.transform.localScale = trans.transform.localScale;
        return go;
    }
    public static GameObject SelfCopy(this GameObject trans)
    {

        GameObject go = GameObject.Instantiate(trans) as GameObject;
        go.transform.parent = trans.transform.parent;
        go.transform.localPosition = trans.transform.localPosition;
        go.transform.localRotation = trans.transform.localRotation;
        go.transform.localScale = trans.transform.localScale;
        return go;
    }
}
