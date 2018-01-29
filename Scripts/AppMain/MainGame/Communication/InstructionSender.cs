using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ZF.Communication
{
    using Global;
    public class InstructionSender : MonoBehaviour
    {
        private Instruction instruction;
        int id;
        bool isSending = false;
        IPAddress iPAddress;
        IPEndPoint endPoint;
        Socket clientSocket;
        public void Init(string _iPAddress, Instruction _instruction, int _id)
        {
            id = _id;
            iPAddress = IPAddress.Parse(_iPAddress);
            endPoint = new IPEndPoint(iPAddress, Global.instructionListeningPort);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            instruction = _instruction;
        }

        #region MonoBehavior Callbacks
        private void Start()
        {
            if(GameState.mode == GameMode.isServer)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if(isSending)
            {
                byte[] sendingBytes = new byte[Global.UdpBytesLength];
                sendingBytes[0] = Global.creditByte;
                sendingBytes[1] = (byte)id;
                instruction.ToByte().CopyTo(sendingBytes, 2);
                clientSocket.SendTo(sendingBytes, endPoint);
            }
        }
        #endregion
        public void StartSending()
        {
            isSending = true;
        }
        public void StopSending()
        {
            isSending = false;
        }
    }

}
