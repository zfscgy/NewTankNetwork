using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace ZF.WholeGame.GameReplay
{
    using ZF.MainGame.Base;
    public class GameReplayer
    {
        private Tank[] AllPlayers;
        private BinaryReader binaryReader;
        private FileStream fileStream;
        private GameSaveFileHeader header = new GameSaveFileHeader();
        public int[] Init(string filePath)
        {           
            if(!File.Exists(filePath))
            {
                return null;
            }
            fileStream = File.Open(filePath, FileMode.Open);
            binaryReader = new BinaryReader(fileStream);
            header.Read(binaryReader);
            return header.PlayerSeats;
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
                        transform.eulerAngles = new Vector3(binaryReader.ReadSingle(), transform.localEulerAngles.y, transform.localEulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyRotationY:
                        transform.eulerAngles = new Vector3(transform.localEulerAngles.x, binaryReader.ReadSingle(), transform.localEulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyRotationZ:
                        transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, binaryReader.ReadSingle());
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
        public void NextFrame()
        {
            for (int i = 0; i < AllPlayers.Length; i++)
            {

            }
        }
    }
}
