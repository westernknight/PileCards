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
    List<int> outCardsHeap = new List<int>();

    public static GameManager instance;
    public GameObject cardPrefab;

    GameObject handCardGrid;
    GameObject outCardGrid;

    GameObject mainMenuButton;

    CardData cardConfig;
    void Awake()
    {
        instance = this;

        //demo
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

        cardConfig = CardData.instance;

    }
    [RPC]
    void InitCards(string json)
    {
        cardNames.Clear();
        cardNames = LitJson.JsonMapper.ToObject<List<string>>(json);

    }

    void Start()
    {
        network = GetComponent<NetworkView>();




        Debug.Log("i'am " + Network.player);


        mainMenuButton = GameObject.Find("mainMenuButton");
        handCardGrid = GameObject.Find("handCardGrid");
        outCardGrid = GameObject.Find("outCardGrid");
        mainMenuButton.SetActive(false);

        if (CardData.instance.cardJsonData != null)
        {
            cardNames.Clear();
            Debug.Log("load cards");
            LitJson.JsonData pa = CardData.instance.cardJsonData;

            for (int i = 0; i < pa.Count; i++)//pa 意思是card 种类
            {
                foreach (string item in ((IDictionary)(pa[i])).Keys)//key意思是 card的每个属性
                {
                    LitJson.JsonData data = pa[i];
                    if (item == "name")
                    {
                        cardNames.Add((string)data[item]);
                    }

                }
            }
            network.RPC("InitCards", RPCMode.AllBuffered, LitJson.JsonMapper.ToJson(cardNames));
        }



        if (Network.isServer)
        {
            mainMenuButton.SetActive(true);


            Random.seed = System.Environment.TickCount;
            pileCards.Clear();
            for (int i = 0; i < cardNames.Count; i++)
            {
                pileCards.Add(i);
            }
            for (int i = 0; i < cardConfig.startCardCount; i++)
            {

                for (int j = 0; j < Network.connections.Length + 1; j++)
                {
                    if (pileCards.Count > 0)
                    {
                        int index = Random.Range(0, pileCards.Count);
                        if (j == Network.connections.Length)
                        {
                            ActionSendCard(Network.player, pileCards[index]);
                            pileCards.RemoveAt(index);
                        }
                        else
                        {
                            network.RPC("ActionSendCard", RPCMode.Others, Network.connections[j], pileCards[index]);
                            pileCards.RemoveAt(index);
                        }
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
    /// <summary>
    /// all client run
    /// </summary>
    /// <param name="player"></param>
    /// <param name="pileIndex"></param>
    [RPC]
    void ActionSendCard(NetworkPlayer player, int pileIndex)
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
    /// <summary>
    /// all client run
    /// </summary>
    /// <param name="player"></param>
    /// <param name="json"></param>
    [RPC]
    void ActionOutCard(NetworkPlayer player, string json)
    {
        Debug.Log("ActionOutCard");
        List<int> outCards = LitJson.JsonMapper.ToObject<List<int>>(json);
        for (int i = 0; i < outCards.Count; i++)
        {
            outCardsHeap.Add(outCards[i]);
        }
        Debug.Log("outCards " + outCards.Count);
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
            while (outCardGrid.transform.childCount > 0)
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
    /// <summary>
    /// server run
    /// </summary>
    /// <param name="player"></param>
    [RPC]
    void RequestSendCard(NetworkPlayer player)
    {
        for (int k = 0; k < cardConfig.sendCardCount; k++)
        {
            if (pileCards.Count > 0)
            {
                int index = Random.Range(0, pileCards.Count);
                network.RPC("ActionSendCard", RPCMode.All, player, pileCards[index]);
                pileCards.RemoveAt(index);
            }
            else if (cardConfig.circulation)
            {

                while (outCardsHeap.Count>0)
                {
                    int index = Random.Range(0, outCardsHeap.Count);
                    pileCards.Add(outCardsHeap[index]);
                    outCardsHeap.RemoveAt(index);
                }
                if (pileCards.Count>0)
                {
                    k--;
                }
                else
                {
                    Debug.Log("no out cards.");
                }
            }
        }
            
        
    }
    
    /// <summary>
    /// server run
    /// </summary>
    /// <param name="player"></param>
    /// <param name="json"></param>
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
            //如果只有server自己一个，server 是不能发送给server的，所以这里实现

            RequestSendCard(Network.player);
        }
        else
        {
            network.RPC("RequestSendCard", RPCMode.Server, Network.player);
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
            RequestOutCard(Network.player, json);
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
