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

public class CardData : MonoBehaviour {

    public static CardData instance;
    public LitJson.JsonData cardJsonData;
    public int startCardCount = 5;
    public int sendCardCount = 1;
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
	void Start()
	{
	
	}
}
