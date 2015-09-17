#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：CardAttribute
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/17/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardAttribute : MonoBehaviour {

    public int index;
    public bool selected;
    /// <summary>
    /// child 
    /// </summary>
    /// <param name="rim"></param>
    public void OnCardClick(GameObject rim)
    {
        rim.GetComponent<Image>().enabled = !rim.GetComponent<Image>().enabled;
        selected = !selected;
    }
   
}
