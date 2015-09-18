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

using System.Collections.Generic;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
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
            File.Copy(excelPath, excelPathWithoutExtension, true);
            FileStream stream = File.Open(excelPathWithoutExtension, FileMode.Open, FileAccess.Read);

            HSSFWorkbook wk = new HSSFWorkbook(stream);
            stream.Close();
            File.Delete(excelPathWithoutExtension);

            ISheet sheet = wk.GetSheetAt(0);

            Debug.Log("Rows count " + sheet.LastRowNum);
            LitJson.JsonData result = new LitJson.JsonData();
            result["buff"] = new LitJson.JsonData();
            List<string> keyName = new List<string>();

            for (int j = 0; j < sheet.LastRowNum; j++)
            {
                if (j == 0)
                {
                    for (int i = 0; i < sheet.GetRow(0).LastCellNum; i++)
                    {
                        ICell cell = sheet.GetRow(0).GetCell(i);
                        if (cell!=null)
                        {
                            keyName.Add(cell.ToString());
                        }
                        else
                        {
                            keyName.Add("");
                        }

                    }
                }
                else
                {
                    LitJson.JsonData childJ = new LitJson.JsonData();
                    //childJ["subBuff"] = new LitJson.JsonData();
                    for (int i = 0; i < keyName.Count; i++)
                    {
                        ICell cell = sheet.GetRow(j).GetCell(i);//mResultSets.Tables[0].Rows[j][i].ToString();
                        if (cell != null)
                        {
                            childJ[keyName[i]] = cell.ToString();
                        }
                        else
                        {
                            childJ[keyName[i]] = null;
                        }
                        
                    }

                    result["buff"].Add(childJ);
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
                    Debug.Log("==================");

                }
            }
            return;

        }
    }
}