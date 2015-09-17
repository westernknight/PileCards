#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：TimeSpeedEditor
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/7/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using UnityEditor;
public class TimeSpeedEditor : EditorWindow {

    
    float time;
    GameObject loadObject = null;
	[MenuItem("Debug/play speed controller")]  
    static void Init()
    {
		TimeSpeedEditor window = (TimeSpeedEditor)EditorWindow.GetWindow(typeof(TimeSpeedEditor));
        window.Show();
        
    }
    void OnGUI()
    {
        //EditorGUILayout.HelpBox("填写数字将跳进数字秒数，例如20秒就跳20秒", MessageType.Info);
		EditorGUILayout.BeginHorizontal();
		
		if (GUILayout.Button("x0.2"))
		{
			Time.timeScale = 0.2f;      
		}
		if (GUILayout.Button("x0.3"))
		{
			Time.timeScale = 0.3f;      
		}
		if (GUILayout.Button("x0.5"))
		{
			Time.timeScale = 0.5f;      
		}
		EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
      
        if (GUILayout.Button("x2"))
        {
			Time.timeScale = 2;      
        }
		if (GUILayout.Button("x3"))
		{
			Time.timeScale = 3;      
		}
		if (GUILayout.Button("x5"))
		{
			Time.timeScale = 5;      
		}
        EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("|>"))
		{
			Time.timeScale = 1;      
		}
		if (GUILayout.Button("||"))
		{
			Time.timeScale = 0;      
		}
		EditorGUILayout.EndHorizontal();
    }
}
