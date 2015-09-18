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
public class startScene : MonoBehaviour {

	public GameObject canvas;
	public GameObject beginButton;
	public Text text;

    FileBrowser fb;
    string jsonFilePath = "";
	void Start()
	{
		switch (Network.peerType) {
		case NetworkPeerType.Server:
			break;
		case NetworkPeerType.Client:
			beginButton.SetActive(false);
			break;
		}
	}

    public GUISkin[] skins;
    public Texture2D file, folder, back, drive;

    public void LoadJson()
    {
        

        fb = new FileBrowser();
        fb.guiSkin = skins[0]; //set the starting skin
        //set the various textures
        fb.fileTexture = file;
        fb.directoryTexture = folder;
        fb.backTexture = back;
        fb.driveTexture = drive;
        //show the search bar
        fb.showSearch = false;
        //search recursively (setting recursive search may cause a long delay)
        fb.searchRecursively = true;
// 
// 
//         FileInfo jsonFile = new FileInfo(Path.GetFileNameWithoutExtension(excelPath) + ".json");
//         StreamReader sr = new StreamReader(jsonFile.OpenRead());
//         string buffDate = sr.ReadLine();
//         sr.Close();
// 
//         LitJson.JsonData arguments = LitJson.JsonMapper.ToObject(buffDate);
//         if (arguments["buff"].IsArray)
//         {
//             Debug.Log("array");
//             LitJson.JsonData pa = arguments["buff"];
//             for (int i = 0; i < pa.Count; i++)
//             {
//                 foreach (string item in ((IDictionary)(pa[i])).Keys)
//                 {
//                     LitJson.JsonData data = pa[i];
//                     Debug.Log(item + " " + data[item]);
//                 }
//                 Debug.Log("");
// 
//             }
//         }
    }
    void OnGUI()
    {
        if (fb != null)
        {
            fb.draw();
        }
    }
	void Update()
	{
        if (fb !=null)
        {
            fb.draw();
            if (fb.outputFile != null)
            {
                Debug.Log(fb.outputFile.ToString());
            }
        }
        

		text.text = "Player Count: "+(Network.connections.Length+1).ToString();//server doesn't counted so +1;
	}
	public void StartBattleScene()
	{
		NetworkTrans.instance.UserLoadLevel ("a0");
	}
}
