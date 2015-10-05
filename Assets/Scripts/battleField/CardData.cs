#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：CardData
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/21/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardData : MonoBehaviour {

    public static CardData instance;
    public LitJson.JsonData cardJsonData;
    public int sendCardCount = 1;
    public int startCardCount = 5;
    
    public bool circulation = true;
    void Awake()
    {
        if (instance!=null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }
    /// <summary>
    /// 比start先调用
    /// </summary>
    void OnLevelWasLoaded()
    {
        if (Application.loadedLevel == 2)
        {
            GameObject.Find("sendCardCountInputField").GetComponent<InputField>().text = sendCardCount.ToString();
            GameObject.Find("sendCardCountInputField").GetComponent<InputField>().onValueChange.AddListener((value) => { sendCardCount = int.Parse(value); });
            GameObject.Find("initCardCountInputField").GetComponent<InputField>().text = startCardCount.ToString();
            GameObject.Find("initCardCountInputField").GetComponent<InputField>().onValueChange.AddListener((value) => { startCardCount = int.Parse(value); });
            GameObject.Find("circulation").GetComponent<Toggle>().isOn = circulation;
            GameObject.Find("circulation").GetComponent<Toggle>().onValueChanged.AddListener((value) => { circulation = value; });
        }
        
    }
  
	void Start()
	{
        
	}
    
}
