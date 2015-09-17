using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;
namespace SocketConnection
{
    public class SocketClient : SocketConnection
    {
        string ip;
        int port;
        bool needReconnect = false;//manager不在线是否重拨

        public bool isConnected = false;
        private bool bKeepAlive = false;
        public Action connected_callback;

        #region KeepAlive设置
        enum EKeepAlive
        {
            NotKeepAlive = 0,
            KeepAlive = 1
        }
        public void SetKeepAlive(bool keepAlive)
        {
            bKeepAlive = keepAlive;
        }
        uint dummy = 0;
        void SetKeepAliveValues(byte[] inOptionValues, EKeepAlive bKeepAlive, uint startTime, uint intervalTime)
        {

            BitConverter.GetBytes((uint)bKeepAlive).CopyTo(inOptionValues, 0);//是否启用Keep-Alive
            BitConverter.GetBytes(startTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));//多长时间开始第一次探测
            BitConverter.GetBytes(intervalTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);//探测时间间隔
        }

        private void SetKeepAlive()
        {
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            SetKeepAliveValues(inOptionValues, EKeepAlive.KeepAlive, 5000, 5000);
            //byte[] inValue = new byte[] { 1, 0, 0, 0, 0x20, 0x4e, 0, 0, 0xd0, 0x07, 0, 0 };// 首次探测时间20 秒, 间隔侦测时间2 秒
            byte[] inValue = new byte[] { 1, 0, 0, 0, 0x88, 0x13, 0, 0, 0xd0, 0x07, 0, 0 };// 首次探测时间5 秒, 间隔侦测时间2 秒
            socket.IOControl(IOControlCode.KeepAliveValues, inValue, null);
        }
        #endregion
        
        public void Start(string ip, int port,bool mustConnectToServer)
        {

            this.ip = ip;
            this.port = port;
            try
            {
                
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);

                if (bKeepAlive)
                {
                    SetKeepAlive();
                }
                if (mustConnectToServer)
                {
                    needReconnect = true;
                }
                Msg("try to connect to server [" + ip + ":" + port + "]");

                socket.BeginConnect(ipep, ConnectCallback, socket);
                Msg("BeginConnect");
                disConnected_callback += () => { isConnected = false; };

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Msg(ex);

            }
        }
        
        private void ConnectCallback(System.IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);
                ar.AsyncWaitHandle.Close();

                socket.BeginReceive(tmpData, 0, tmpData.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                Msg("connect  [" + ip + ":" + port + "] success");
                isConnected = true;
                if (connected_callback!=null)
                {
                    connected_callback();
                }
                
            }
            catch (System.Exception ex)
            {
                Msg("ConnectCallback failed: " + ex.Message);
                isConnected = false;
              
                if (needReconnect == true)
                {
                    Thread.Sleep(1000);//sleep 1 second
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);
                    socket.BeginConnect(
                    ipep,
                    new System.AsyncCallback(ConnectCallback),
                    socket);
                }

            }
        }

        public void SendPackage(string jsonData)
        {
            Debug.Log("SendPackage");
            if (socket != null)
            {
                Debug.Log(socket.Connected);
                if (socket.Connected)
                {
                    Debug.Log(jsonData);
                    byte[] data = Encoding.UTF8.GetBytes(jsonData);
                    byte[] send_data_with_length = BuildPack(BitConverter.GetBytes(data.Length), data);
                    socket.Send(send_data_with_length);
                }

            }

        }
        public void SendPackage(byte[] data)
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    byte[] send_data_with_length = BuildPack(BitConverter.GetBytes(data.Length), data);
                    socket.Send(send_data_with_length);
                }

            }

        }
    }
}
