using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace BaseFramework
{
    public class SocketHelper : Singleton<SocketHelper>
    {
        private string serverIP = "127.0.0.1";
        private int serverPort = 996;

        private const int RECEIVE_MAX_NUM = 1024;
        private byte[] receiveArray;

        private Socket socket;

        public delegate void ConnectCallback();

        public event ConnectCallback ConnectSucceededCallback;
        public event ConnectCallback ConnectFailedCallback;
        public event ConnectCallback ConnectCloseCallback;

        private byte[] cacheData = new byte[0];

        public override void Init()
        {
            base.Init();
        }

        public override void Release()
        {
            CloseConnect();

            base.Release();
        }

        public void InitConnect()
        {
            Connect();
        }

        public void CloseConnect()
        {
            DisConnect();
        }

        private void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress address = IPAddress.Parse(serverIP);
            IPEndPoint endPoint = new IPEndPoint(address, serverPort);
            IAsyncResult result = socket.BeginConnect(endPoint, new AsyncCallback(ConnectedCallback), socket);
        }

        private void DisConnect()
        {
            if (socket != null && socket.Connected)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket = null;
                }
                catch (Exception e)
                {
                    MDebug.LogError(e.StackTrace);
                    socket = null;
                    return;
                }
                ConnectCloseCallback?.Invoke();
            }
        }

        private void ConnectedCallback(IAsyncResult ar)
        {
            try
            {
                MDebug.Log("Connected");
                socket.EndConnect(ar);
                ConnectSucceededCallback?.Invoke();
                Receive();
            }
            catch (Exception e)
            {
                MDebug.LogError(e.StackTrace);
                ConnectFailedCallback?.Invoke();
                return;
            }
        }

        public void Send(string data)
        {
            Send(socket, AssemblyMsg(data));
        }

        private byte[] AssemblyMsg(string data)
        {
            byte[] msgData = Encoding.UTF8.GetBytes(data);

            return AssemblyMsg(msgData);
        }

        private byte[] AssemblyMsg(byte[] data)
        {
            byte[] msgData = new byte[data.Length + ReceiveMsgData.HEADLENGTH];
            MemoryStream ms = new MemoryStream(msgData);
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write('^');
            bw.Write((byte)1);
            bw.Write((byte)2);
            bw.Write(data.Length);
            bw.Write(data);

            bw.Close();
            ms.Close();

            return msgData;
        }

        private void Send(Socket socket, byte[] data)
        {
            if (socket == null || !socket.Connected || data == null)
            {
                Debug.LogError("Socket Error Or Data Is Null");
                return;
            }

            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
                {
                    int length = socket.EndSend(ar);
                    Debug.Log($"Send End , {length}");
                }, null);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void Receive()
        {
            try
            {
                if (socket == null || !socket.Connected)
                {
                    throw new Exception("Socket Error");
                }

                receiveArray = new byte[RECEIVE_MAX_NUM];
                socket.BeginReceive(receiveArray, 0, RECEIVE_MAX_NUM, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (Exception e)
            {
                MDebug.LogError(e.StackTrace);
                DisConnect();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            int length = socket.EndReceive(ar);
            if (length > 0)
            {
                AnalysisReceiveData(receiveArray, length);
                Receive();
            }
        }

        private void AnalysisReceiveData(byte[] receiveData, int length)
        {
            cacheData = CombineBytes(cacheData, 0, cacheData.Length, receiveData, 0, length);

            AnalysisReceiveData();
        }

        private void AnalysisReceiveData()
        {
            if (cacheData.Length >= ReceiveMsgData.HEADLENGTH)
            {
                if (CheckMsgMark(cacheData))
                {
                    ReceiveMsgData receiveMsgData = new ReceiveMsgData(cacheData);
                    if (receiveMsgData.IsCompleteMsg)
                    {
                        ReceiveMsg(receiveMsgData);

                        if (receiveMsgData.MoreData != null)
                        {
                            cacheData = receiveMsgData.MoreData;
                            AnalysisReceiveData();
                        }
                        else
                        {
                            cacheData = new byte[0];
                        }
                    }
                }
                else
                {
                    cacheData = new byte[0];
                }
            }
        }

        private byte[] CombineBytes(byte[] firstBytes, int firstIndex, int firstLength, byte[] secondBytes, int secondIndex, int secondLength)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(firstBytes, firstIndex, firstLength);
            ms.Write(secondBytes, secondIndex, secondLength);
            byte[] result = ms.ToArray();
            ms.Close();
            return result;
        }

        private bool CheckMsgMark(byte[] data)
        {
            char mark = Convert.ToChar(data[0]);
            return mark == ReceiveMsgData.MARK;
        }

        private void ReceiveMsg(ReceiveMsgData data)
        {
            Debug.Log($"Receive:{Encoding.UTF8.GetString(data.MessageData)}");
        }
    }

    public class ReceiveMsgData
    {
        public const int HEADLENGTH = 7;
        public const char MARK = '^';

        private char mark;
        private byte command = 0;
        public byte Command => command;
        private byte param = 0;
        public byte Param => param;
        private int dataLength = 0;
        public int DataLength => dataLength;
        private byte[] messageData;
        public byte[] MessageData => messageData;
        private byte[] moreData;
        public byte[] MoreData => moreData;

        private bool isCompleteMsg = false;
        public bool IsCompleteMsg => isCompleteMsg;

        public ReceiveMsgData(byte[] data)
        {
            if (data == null || data.Length < HEADLENGTH)
            {
                return;
            }

            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);

            try
            {
                mark = br.ReadChar();
                command = br.ReadByte();
                param = br.ReadByte();
                dataLength = br.ReadInt32();
                if ((data.Length - HEADLENGTH) >= dataLength)
                {
                    messageData = br.ReadBytes(dataLength);
                    isCompleteMsg = true;
                    if (data.Length - HEADLENGTH - dataLength > 0)
                    {
                        moreData = br.ReadBytes(data.Length - HEADLENGTH - dataLength);
                    }
                }
                else
                {
                    messageData = br.ReadBytes(data.Length - HEADLENGTH);
                }
            }
            catch (Exception e)
            {

            }

            br.Close();
            ms.Close();
        }
    }
}
