using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;
namespace ZF.Server.InstructionServer
{
    using ZF.Communication;
    using ZF.Global;
    public class InstructionReceivingException : Exception
    {
        public InstructionReceivingException()
        { }

        public InstructionReceivingException(string message) 
        : base(message)
        { }

        public InstructionReceivingException(string message, Exception innerException)
        : base (message, innerException)
        { }
    }

    public class InstructionReceiver : MonoBehaviour
    {
        private UdpClient udpListener;
        private IPEndPoint endPoint;
        private bool isListening = false;
        public Instruction[] ReceivedInstructions = new Instruction[Global.playerPerRoom];
        #region Monobehavior Callbacks
        // Use this for initialization
        void Start()
        {
            SceneManager.sceneUnloaded += ExitMainGame;            
        }

        // Update is called once per frame
        void Update()
        {
            while (udpListener.Available > 0) 
            {                
                byte[] receivedBytes = udpListener.Receive(ref endPoint);
                if(receivedBytes[1] > Global.playerPerRoom || receivedBytes[1] == 0 || receivedBytes[0] != 0x1f)
                {
                    throw new InstructionReceivingException("Error: Illeagle Instruction received!");
                }
                byte[] instructionBytes = new byte[Global.UdpBytesLength - 2];
                Array.Copy(receivedBytes, 2, instructionBytes, 0, Global.UdpBytesLength - 2);
                ReceivedInstructions[receivedBytes[1]].BytesToInstruction(instructionBytes);
            }
        }
        #endregion
        public void Init()
        {
            enabled = true;
            for (int i = 0; i < ReceivedInstructions.Length; i++)
            {
                ReceivedInstructions[i] = new Instruction();
            }
            udpListener = new UdpClient(Global.instructionListeningPort);
            endPoint = new IPEndPoint(IPAddress.Any, Global.instructionListeningPort);
        }
        public void StartListening()
        {
            isListening = true;
        }
        public void StopListen()
        {
            isListening = false;
        }
        public void CloseConnection()
        {
            isListening = false;
            udpListener.Close();
        }

        public void ExitMainGame(Scene scene)
        {
            CloseConnection();
        }
    }
}
