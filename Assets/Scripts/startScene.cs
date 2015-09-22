#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：startScene
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/16/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class startScene : MonoBehaviour {

	public GameObject canvas;
    public GameObject setting;
    public GameObject ipComponent;
	public Text text;
    EventSystem es;
    FileBrowser fb;
    string excelPath = "";
    void Awake()
    {
        Toggle toggle = GameObject.Find("circulation").GetComponent<Toggle>();
        toggle.isOn = CardData.instance.circulation;
    }
	void Start()
	{
        es = EventSystem.current;
		switch (Network.peerType) {
		case NetworkPeerType.Server:
                ipComponent.GetComponent<Text>().text = "IP: " + Network.player.ipAddress;
			break;
		case NetworkPeerType.Client:
            ipComponent.SetActive(false);
            setting.SetActive(false);
			break;
		}
	}
    public void SetCardDataCirculation(Toggle toggle)
    {
        CardData.instance.circulation = toggle.isOn;
    }

    public GUISkin[] skins;
    public Texture2D file, folder, back, drive;

    public void LoadJson()
    {
        es.enabled = false;


        fb = new FileBrowser(Directory.GetCurrentDirectory());
        fb.guiSkin = skins[0]; //set the starting skin
        //set the various textures
        fb.fileTexture = file;
        fb.directoryTexture = folder;
        fb.backTexture = back;
        fb.driveTexture = drive;
        fb.searchPattern = "*.xls";
        //show the search bar
        fb.showSearch = false;
        //search recursively (setting recursive search may cause a long delay)
        fb.searchRecursively = true;

    }

    void AnylizeJsonFile(string path)
    {
        if (Path.GetFileName(excelPath).Contains(".xls"))
        {
            string excelPathWithoutExtension = Path.GetFileNameWithoutExtension(excelPath) + "-copy.xls";
            excelPathWithoutExtension = Path.GetDirectoryName(excelPath) + "/" + excelPathWithoutExtension;
      
            File.Copy(excelPath, excelPathWithoutExtension, true);
            
            FileStream stream = File.Open(excelPathWithoutExtension, FileMode.Open, FileAccess.Read);
            Debug.Log("===================");
            HSSFWorkbook wk = new HSSFWorkbook(stream);
            stream.Close();
            File.Delete(excelPathWithoutExtension);
            Debug.Log("===================");
            ISheet sheet = wk.GetSheetAt(0);
            Debug.Log("===================");
            Debug.Log("Rows count " + sheet.LastRowNum);
            LitJson.JsonData result = new LitJson.JsonData();
            result["attribute"] = new LitJson.JsonData();
            List<string> keyName = new List<string>();
            for (int j = 0; j < sheet.LastRowNum; j++)
            {
               
                    if (j == 0)
                    {
                        for (int i = 0; i < sheet.GetRow(0).LastCellNum; i++)
                        {
                            ICell cell = sheet.GetRow(0).GetCell(i);
                            if (cell != null)
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
                        for (int i = 0; i < keyName.Count; i++)
                        {
                            ICell cell = sheet.GetRow(j).GetCell(i);
                            if (cell != null)
                            {
                                childJ[keyName[i]] = cell.ToString();
                            }
                            else
                            {
                                childJ[keyName[i]] = null;
                            }

                        }

                        result["attribute"].Add(childJ);
                    }
                
            }

            if (result["attribute"].IsArray)
            {
                LitJson.JsonData pa = result["attribute"];
                if (CardData.instance)
                {
                    CardData.instance.cardJsonData = pa;
                }
                for (int i = 0; i < pa.Count; i++)//pa 意思是card 种类
                {
                    foreach (string item in ((IDictionary)(pa[i])).Keys)//key意思是 card的每个属性
                    {
                        LitJson.JsonData data = pa[i];
                        //Debug.Log(item + " " + data[item]);
                    }
                    //Debug.Log("==================");

                }
            }
    
        }
        
    }
    void OnGUI()
    {
        if (fb != null)
        {
     
            if (fb.draw())
            {
                if (fb.outputFile!=null)
                {

                    excelPath = fb.outputFile.Directory.FullName+"/"+fb.outputFile.Name;
                 
                    Debug.Log(excelPath);
                    AnylizeJsonFile(excelPath);                    
                   
                }
                fb = null;
                es.enabled = true;
            }
            
        }
    }

	void Update()
	{
      

		text.text = "Player Count: "+(Network.connections.Length+1).ToString();//server doesn't counted so +1;
	}
	public void StartBattleScene()
	{
		NetworkTrans.instance.UserLoadLevel ("a0");
	}
}
