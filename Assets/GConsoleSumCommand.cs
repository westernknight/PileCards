#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：GConsoleSumCommand
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/22/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using System.Linq;

public class GConsoleSumCommand : MonoBehaviour {

    void Start()
    {
        GConsole.AddCommand("sum", "Adds numbers separated by \"+\"", Sum);
    }

    string Sum(string param)
    {
        try
        {
            //First split the string into an array of strings separated by +, then parse the individual parts and sum!
            return "SUM: " + param.Split('+').Sum(s => float.Parse(s));
        }
        catch
        {
            return "Invalid input, remember you can only have numbers and \"+\"!";
        }
    }
}
