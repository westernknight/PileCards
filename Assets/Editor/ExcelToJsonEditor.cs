#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：ExcelToJson
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/17/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System.Collections.Generic;
public class ExcelToJsonEditor : EditorWindow
{


    string excelPath = "";
    [MenuItem("Tools/ExcelToJson ")]
    static void Init()
    {
        ExcelToJsonEditor window = (ExcelToJsonEditor)EditorWindow.GetWindow(typeof(ExcelToJsonEditor));
        window.Show();

    }
    void OnGUI()
    {
        EditorGUILayout.LabelField("Excel Path:");
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("..."))
        {
            excelPath = EditorUtility.OpenFilePanel(
                    "Excel Path",
                    "",
                    "xls");

        }
        excelPath = EditorGUILayout.TextField(excelPath);
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Convert"))
        {
            string excelPathWithoutExtension = Path.GetFileNameWithoutExtension(excelPath) + "-copy.xls";
            File.Copy(excelPath, excelPathWithoutExtension,true);
            FileStream stream = File.Open(excelPathWithoutExtension, FileMode.Open, FileAccess.Read);

            IExcelDataReader exc = ExcelReaderFactory.CreateBinaryReader(stream);
            stream.Close();
            File.Delete(excelPathWithoutExtension);
            DataSet mResultSets = exc.AsDataSet();


            Debug.Log("mResultSets.Tables[0].Columns.Count " + mResultSets.Tables[0].Columns.Count);
            Debug.Log("mResultSets.Tables[0].Rows.Count " + mResultSets.Tables[0].Rows.Count);

            LitJson.JsonData result = new LitJson.JsonData();
            result["buff"] = new LitJson.JsonData();
            List<string> keyName = new List<string>();

            for (int j = 0; j < mResultSets.Tables[0].Rows.Count; j++)
            {
                if (j == 0)
                {
                    //first row is head brand
                }
                else
                {
                    LitJson.JsonData childJ = new LitJson.JsonData();
                    //childJ["subBuff"] = new LitJson.JsonData();
                    for (int i = 0; i < keyName.Count; i++)
                    {
                        childJ[keyName[i]] = mResultSets.Tables[0].Rows[j][i].ToString();
                    }

                    result["buff"].Add(childJ);
                }
                if (j == 0)
                {
                    for (int i = 0; i < mResultSets.Tables[0].Columns.Count; i++)
                    {
                        keyName.Add(mResultSets.Tables[0].Rows[j][i].ToString());

                    }
                }
            }

            FileInfo jsonFile = new FileInfo(Path.GetFileNameWithoutExtension(excelPath) + ".json");
            StreamWriter sw = new StreamWriter(jsonFile.Create());
            sw.WriteLine(result.ToJson());
            sw.Close();
            StreamReader sr = new StreamReader(jsonFile.OpenRead());
            string buffDate = sr.ReadLine();
            sr.Close();
            //read json
            LitJson.JsonData arguments = LitJson.JsonMapper.ToObject(buffDate);
            if (arguments["buff"].IsArray)
            {
                Debug.Log("array");
                LitJson.JsonData pa = arguments["buff"];
                for (int i = 0; i < pa.Count; i++)
                {
                    foreach (string item in ((IDictionary)(pa[i])).Keys)
                    {
                        LitJson.JsonData data = pa[i];
                        Debug.Log(item + " " + data[item]);
                    }
                    Debug.Log("");

                }
            }
        }
    }
}