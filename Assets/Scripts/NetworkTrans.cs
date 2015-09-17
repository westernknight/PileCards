#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：NetworkTrans
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/16/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;

public class NetworkTrans : MonoBehaviour {

	public static NetworkTrans instance = null;
	NetworkView network;
	private int serverPort;
	private string serverIP = "127.0.0.1";
	int lastLevelPrefix = 0;
	public string loadSceneName;//asyn load scene offer name
	void Awake()
	{
		instance = this;
		serverPort = 10000;
	}
	void Start()
	{
		network = GetComponent<NetworkView>();
		DontDestroyOnLoad(gameObject);

	}
	public void StartServer()
	{
		NetworkConnectionError error = Network.InitializeServer(6, serverPort, true);
		Debug.Log("server status: " + error);
		Application.LoadLevel("zjj_startScene");
	}
	public void StartConnect()
	{
		NetworkConnectionError error = Network.Connect(serverIP, serverPort);
		Debug.Log("connect status: " + error);
		if (error == NetworkConnectionError.NoError)
		{
			Invoke("OnConnect", 0.5f);
		}
	}
	void OnConnect()
	{
		
		if (Network.isClient)
		{
			Debug.Log("OnConnect");
			network.RPC("ClientConnected", RPCMode.All);
		}
		else
		{
			Debug.Log("Connect error");

		}
		
	}
	[RPC]
	void ClientConnected()
	{
		Debug.Log("ClientConnected");
		if (Network.isServer)
		{
			network.RPC("LoadLevelIEnumerator", RPCMode.AllBuffered, "zjj_startScene", lastLevelPrefix + 1);
		}
	}
	[RPC]
	void LoadLevelIEnumerator(string level, int levelPrefix)
	{
		lastLevelPrefix = levelPrefix;
		
		Network.SetLevelPrefix(levelPrefix);
		
		
		Application.LoadLevel(level);
	}
	[RPC]
	void LoadLevelIEnumeratorAsyn(string level, int levelPrefix)
	{
		lastLevelPrefix = levelPrefix;
		
		Network.SetLevelPrefix(levelPrefix);
		
		
		Application.LoadLevelAsync(level);
	}
	
	[RPC]
	void SynchronizeTimeScale(float timeScale)
	{
		Time.timeScale = timeScale;
	}
	public void UserLoadLevel(string name)
	{
		if (Network.isServer)
		{
			Time.timeScale = 1;
			network.RPC("SynchronizeTimeScale", RPCMode.AllBuffered, 1f);
			network.RPC("LoadLevelIEnumerator", RPCMode.AllBuffered, name, lastLevelPrefix + 1);
		}
	}
	public void UserLoadLevelAsyn(string name)
	{
		if (Network.isServer)
		{
			Time.timeScale = 1;
			network.RPC("SynchronizeTimeScale", RPCMode.AllBuffered, 1f);
			network.RPC("LoadLevelIEnumeratorAsyn", RPCMode.AllBuffered, name, lastLevelPrefix + 1);
		}
	}
	void Update()
	{
		switch (Network.peerType)
		{
			//禁止客户端连接运行, 服务器未初始化    
			case NetworkPeerType.Disconnected:            
			break;
			//运行于服务器端    
			case NetworkPeerType.Server:
		
			
			//OnServer();
			break;
			//运行于客户端    
			case NetworkPeerType.Client:
			//OnConnect();

			
			break;
			//正在尝试连接到服务器    
			case NetworkPeerType.Connecting:
			break;
		}
	}

	void OnDisconnectedFromServer()
	{
		Time.timeScale = 1;
		Debug.Log("OnDisconnectedFromServer");

		Application.LoadLevel("zjj_connection");

		
		
	}
}
