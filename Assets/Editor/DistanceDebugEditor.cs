#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：DistanceDebugEditor
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/9/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using UnityEditor;
public class DistanceDebugEditor : EditorWindow
{


    float time;

    GameObject firstObject;
    GameObject secondObject;

    GameObject lastFirstObject;
    GameObject lastSecondObject;
    float dis = 0;
    [MenuItem("Debug/计量2物体位置 ")]
    static void Init()
    {
        DistanceDebugEditor window = (DistanceDebugEditor)EditorWindow.GetWindow(typeof(DistanceDebugEditor));
        window.Show();

    }
    void OnGUI()
    {
        EditorGUILayout.HelpBox("分别拖入两个对象", MessageType.Info);
        //EditorGUILayout.BeginHorizontal();
        firstObject = EditorGUILayout.ObjectField("first", firstObject, typeof(GameObject), true) as GameObject;
        secondObject = EditorGUILayout.ObjectField("second", secondObject, typeof(GameObject), true) as GameObject;
        if (lastFirstObject!=firstObject||lastSecondObject!=secondObject)
        {
            lastFirstObject = firstObject;
            lastSecondObject = secondObject;
            if (firstObject!=null&& secondObject!=null)
            {
                dis = Vector3.Distance(firstObject.transform.position, secondObject.transform.position);
            }
        }
        dis = EditorGUILayout.FloatField("distance:", dis) ;
        //EditorGUILayout.EndHorizontal();
    }
}