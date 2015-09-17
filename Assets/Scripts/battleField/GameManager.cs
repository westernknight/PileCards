#region 模块信息
/*----------------------------------------------------------------
// Copyright (C) 2015 广州，蓝弧
//
// 模块名：GameManager
// 创建者：张嘉俊
// 修改者列表：
// 创建日期：9/16/2015
// 模块描述：
//----------------------------------------------------------------*/
#endregion


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    NetworkView network;
    List<string> cardNames = new List<string>();
    List<int> pileCards = new List<int>();

    public int startCardCount = 5;
    public int sendCardCount = 1;
    public static GameManager instance;
    public GameObject cardPrefab;

    GameObject handCardGrid;
    GameObject outCardGrid;

    GameObject mainMenuButton;
    void Awake()
    {
        instance = this;

        cardNames.Add("a1");
        cardNames.Add("a2");
        cardNames.Add("a3");
        cardNames.Add("a4");
        cardNames.Add("a5");
        cardNames.Add("a6");
        cardNames.Add("a7");
        cardNames.Add("a8");
        cardNames.Add("a9");

        cardNames.Add("b1");
        cardNames.Add("b2");
        cardNames.Add("b3");
        cardNames.Add("b4");
        cardNames.Add("b5");
        cardNames.Add("b6");
        cardNames.Add("b7");
        cardNames.Add("b8");
        cardNames.Add("b9");


    }
    void Start()
    {
        network = GetComponent<NetworkView>();
        Debug.Log("i'am " + Network.player);
   
        for (int i = 0; i < cardNames.Count; i++)
        {
            pileCards.Add(i);
        }
        mainMenuButton = GameObject.Find("mainMenuButton");
        handCardGrid = GameObject.Find("handCardGrid");
        outCardGrid = GameObject.Find("outCardGrid");
        mainMenuButton.SetActive(false);
        if (Network.isServer)
        {
            mainMenuButton.SetActive(true);

            Random.seed = System.Environment.TickCount;
            for (int i = 0; i < startCardCount; i++)
            {
                if (pileCards.Count <= 0)
                {
                    break;
                }
                for (int j = 0; j < Network.connections.Length + 1; j++)
                {
                    if (pileCards.Count <= 0)
                    {
                        break;
                    }
                    int index = Random.Range(0, pileCards.Count);
                    if (j == Network.connections.Length)
                    {
                        ActionSendOneCard(Network.player, pileCards[index]);
                        pileCards.RemoveAt(index);
                    }
                    else
                    {
                        network.RPC("ActionSendOneCard", RPCMode.Others, Network.connections[j], pileCards[index]);
                        pileCards.RemoveAt(index);
                    }


                }
            }
        }



    }
    [RPC]
    void RandomNumber(int seed)
    {
        Random.seed = seed;
    }
    [RPC]
    void ActionSendOneCard(NetworkPlayer player, int pileIndex)
    {

        if (player == Network.player)
        {
            Debug.Log("card: " + pileIndex);
           
            GameObject go = GameObject.Instantiate(cardPrefab) as GameObject;
            go.GetComponentInChildren<Text>().text = cardNames[pileIndex];
            go.GetComponent<CardAttribute>().index = pileIndex;
            go.transform.SetParent(handCardGrid.transform, false);
        }
    }
    [RPC]
    void ActionOutCard(NetworkPlayer player, string json)
    {
        Debug.Log("ActionOutCard");
        List<int> outCards = LitJson.JsonMapper.ToObject<List<int>>(json);
        Debug.Log("outCards "+outCards.Count);
        if (player == Network.player)
        {
            //selection out 
            for (int i = 0; i < outCards.Count; i++)
            {
                for (int j = 0; j < handCardGrid.transform.childCount; j++)
                {
                    if (handCardGrid.transform.GetChild(j).GetComponent<CardAttribute>().index == outCards[i])
                    {
                        Transform t = handCardGrid.transform.GetChild(j);
                        t.SetParent(null, false);
                        Destroy(t.gameObject);
                        break;
                    }
                }
            }
        }
        {
            while (outCardGrid.transform.childCount>0)
            {
                Transform t = outCardGrid.transform.GetChild(0);
                t.SetParent(null, false);
                Destroy(t.gameObject);
            }
            
        }
        {
            for (int i = 0; i < outCards.Count; i++)
            {
                GameObject go = GameObject.Instantiate(cardPrefab) as GameObject;
                go.GetComponentInChildren<Text>().text = cardNames[outCards[i]];
                go.GetComponent<CardAttribute>().index = outCards[i];
                go.transform.SetParent(outCardGrid.transform, false);
            }
        }
    }
    [RPC]
    void RequestSendOneCard(NetworkPlayer player)
    {
        if (pileCards.Count > 0)
        {
            int index = Random.Range(0, pileCards.Count);
            network.RPC("ActionSendOneCard", RPCMode.All, player, pileCards[index]);
            pileCards.RemoveAt(index);
        }
    }
    [RPC]
    void RequestOutCard(NetworkPlayer player, string json)
    {
        Debug.Log(json);
        Debug.Log("RequestOutCard");
        network.RPC("ActionOutCard", RPCMode.All, player, json);
    }
    public void OnClickGetCardButton()
    {
        if (Network.isServer)
        {
            if (pileCards.Count > 0)
            {
                int index = Random.Range(0, pileCards.Count);
                network.RPC("ActionSendOneCard", RPCMode.All, Network.player, pileCards[index]);
                pileCards.RemoveAt(index);
            }
        }
        else
        {
            network.RPC("RequestSendOneCard", RPCMode.Server, Network.player);
        }

    }
    public void OnClickOutCardButton()
    {
        List<int> outCards = new List<int>();

        for (int i = 0; i < handCardGrid.transform.childCount; i++)
        {
            if (handCardGrid.transform.GetChild(i).GetComponent<CardAttribute>().selected)
            {
                outCards.Add(handCardGrid.transform.GetChild(i).GetComponent<CardAttribute>().index);
            }
            
        }
        Debug.Log("OnClickOutCardButton");
        string json = LitJson.JsonMapper.ToJson(outCards);
        Debug.Log(json);
        if (Network.isServer)
        {
            network.RPC("ActionOutCard", RPCMode.All, Network.player, json);
        }
        else
        {
            network.RPC("RequestOutCard", RPCMode.Server, Network.player, json);
        }
    }
    public void MainScene()
    {
        NetworkTrans.instance.UserLoadLevel("zjj_startScene");
    }
}
