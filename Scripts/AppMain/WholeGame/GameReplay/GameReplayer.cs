using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
namespace ZF.WholeGame.GameReplay
{
    using ZF.MainGame.Base;
    using Communication;
    public class GameReplayer : MonoBehaviour
    {
        public GameObject tankPrefab;
        public GameObject serverCameraPrefab;
        public InputManager inputManager;
        public Text[] PlayerInfoTexts;
        private Tank[] AllPlayers;
        private BinaryReader binaryReader;
        private FileStream fileStream;
        private Camera serverCamera;
        private GameSaveFileHeader header = new GameSaveFileHeader();

        private void Update()
        {
            NextFrame();
        }

        public bool Init(string filePath)
        {           
            if(!File.Exists(filePath))
            {
                return false;
            }
            fileStream = File.Open(filePath, FileMode.Open);
            binaryReader = new BinaryReader(fileStream);
            header.Read(binaryReader);
            AllPlayers = new Tank[Global.Global.playerPerRoom];
            for(int i = 0; i< Global.Global.playerPerRoom; i++)
            {
                AllPlayers[i] = Instantiate(tankPrefab, i * new Vector3(-100,-100,10), Quaternion.identity).GetComponentInChildren<Tank>();
                AllPlayers[i].GetComponent<Rigidbody>().isKinematic = true;
                AllPlayers[i].motion.SetMode(TankMode.None);
            }
            ServerCameraController serverCameraController = Instantiate(serverCameraPrefab).GetComponent<ServerCameraController>();
            serverCameraController.Init(inputManager);
            serverCamera = serverCameraController.GetComponent<Camera>();
            enabled = true;
            return true;
        }
        public void ReadTransform(Transform transform, TransformRecordMode mode, bool isLocal = false)
        {
            if (!isLocal)
            {
                switch (mode)
                {
                    case TransformRecordMode.All:
                        transform.position = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        transform.eulerAngles = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        break;
                    case TransformRecordMode.OnlyPosition:
                        transform.position = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        break;
                    case TransformRecordMode.OnlyRotationX:
                        transform.eulerAngles = new Vector3(binaryReader.ReadSingle(), transform.eulerAngles.y, transform.eulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyRotationY:
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, binaryReader.ReadSingle(), transform.eulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyRotationZ:
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, binaryReader.ReadSingle());
                        break;
                }
            }
            else
            {
                switch (mode)
                {
                    case TransformRecordMode.All:
                        transform.localPosition = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        transform.localEulerAngles = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        break;
                    case TransformRecordMode.OnlyPosition:
                        transform.localPosition = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        break;
                    case TransformRecordMode.OnlyRotationX:
                        transform.localEulerAngles = new Vector3(binaryReader.ReadSingle(), transform.localEulerAngles.y, transform.localEulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyRotationY:
                        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, binaryReader.ReadSingle(), transform.localEulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyRotationZ:
                        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, binaryReader.ReadSingle());
                        break;
                }
            }
        }

        public void ReadTank(Tank tank)
        {
            ReadTransform(tank.transform, TransformRecordMode.All);
            ReadTransform(tank.motion.tankComponents.turret.transform, TransformRecordMode.OnlyRotationY, true);
            ReadTransform(tank.motion.tankComponents.gun.transform, TransformRecordMode.OnlyRotationX, true);
        }

        public void NextFrame()
        {
            for (int i = 0; i < AllPlayers.Length; i++)
            {
                ReadTank(AllPlayers[i]);
                PlayerInfoTexts[i].rectTransform.position =
                    serverCamera.WorldToScreenPoint(AllPlayers[i].transform.position + new Vector3(0, 5, 0)) + new Vector3(80, -30);
                if(PlayerInfoTexts[i].rectTransform.position.z > 0)
                {
                    PlayerInfoTexts[i].text = i.ToString() + ":" + binaryReader.ReadInt32();
                }
                else
                {
                    binaryReader.ReadInt32();
                    PlayerInfoTexts[i].text = "";
                }
            }
            int nHit = binaryReader.ReadInt32();
            for (int i = 0; i < nHit; i++)
            {
                binaryReader.ReadInt32();
                binaryReader.ReadInt32();
                binaryReader.ReadInt32();
            }
        }
    }
}
