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
public class startScene : MonoBehaviour {

	public GameObject canvas;
	public GameObject beginButton;
	public Text text;
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
	void Update()
	{
		text.text = "Player Count: "+(Network.connections.Length+1).ToString();//server doesn't counted so +1;
	}
	public void StartBattleScene()
	{
		NetworkTrans.instance.UserLoadLevel ("a0");
	}
}
