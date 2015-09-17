#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：loadingScene
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/16/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class loadingScene : MonoBehaviour {

	public Image loadingImage;
	void Start()
	{
		LeanTween.value(loadingImage.gameObject, 255, 100, 2).setOnUpdate((float value) =>
		                                                                  {
			Color color = Color.white;
			color.a = value/255.0f;
			loadingImage.color = color;
		}).setLoopPingPong();
		StartCoroutine (startLoadDelay());
	}
	IEnumerator startLoadDelay()
	{
		yield return new WaitForEndOfFrame ();
		yield return new WaitForEndOfFrame ();
		NetworkTrans.instance.UserLoadLevelAsyn(NetworkTrans.instance.loadSceneName);

	}

}
