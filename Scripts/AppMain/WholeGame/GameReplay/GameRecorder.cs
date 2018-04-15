using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.WholeGame.GameReplay
{
    using ZF.MainGame.Base;
    using ZF.MainGame.Stats;
    //It is a recorder, recording all player tanks' states during the game
    public class GameRecorder
    {
        private FileStream fstream;
        private BinaryWriter binaryWriter;
        private Tank[] AllPlayers;
        private TankStat[] AllPlayerStats;
        public bool CreateFile(string filename)
        {
            if(File.Exists(filename))
            {
                return false;
            }
            fstream = File.Open(filename, FileMode.Create);
            binaryWriter = new BinaryWriter(fstream);
            return true;
        }
        public void WriteSingleInt(int value)
        {
            binaryWriter.Write(value);
        }
        public void RecordTransform(Transform transform, TransformRecordMode mode, bool isLocal = false)
        {
            if (!isLocal)
            {
                switch (mode)
                {
                    case TransformRecordMode.All:
                        binaryWriter.Write(transform.position.x);
                        binaryWriter.Write(transform.position.y);
                        binaryWriter.Write(transform.position.z);
                        binaryWriter.Write(transform.eulerAngles.x);
                        binaryWriter.Write(transform.eulerAngles.y);
                        binaryWriter.Write(transform.eulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyPosition:
                        binaryWriter.Write(transform.position.x);
                        binaryWriter.Write(transform.position.y);
                        binaryWriter.Write(transform.position.z);
                        break;
                    case TransformRecordMode.OnlyRotationX:
                        binaryWriter.Write(transform.eulerAngles.x);
                        break;
                    case TransformRecordMode.OnlyRotationY:
                        binaryWriter.Write(transform.eulerAngles.y);
                        break;
                    case TransformRecordMode.OnlyRotationZ:
                        binaryWriter.Write(transform.eulerAngles.z);
                        break;
                }
            }
            else
            {
                switch (mode)
                {
                    case TransformRecordMode.All:
                        binaryWriter.Write(transform.localPosition.x);
                        binaryWriter.Write(transform.localPosition.y);
                        binaryWriter.Write(transform.localPosition.z);
                        binaryWriter.Write(transform.localEulerAngles.x);
                        binaryWriter.Write(transform.localEulerAngles.y);
                        binaryWriter.Write(transform.localEulerAngles.z);
                        break;
                    case TransformRecordMode.OnlyPosition:
                        binaryWriter.Write(transform.localPosition.x);
                        binaryWriter.Write(transform.localPosition.y);
                        binaryWriter.Write(transform.localPosition.z);
                        break;
                    case TransformRecordMode.OnlyRotationX:
                        binaryWriter.Write(transform.localEulerAngles.x);
                        break;
                    case TransformRecordMode.OnlyRotationY:
                        binaryWriter.Write(transform.localEulerAngles.y);
                        break;
                    case TransformRecordMode.OnlyRotationZ:
                        binaryWriter.Write(transform.localEulerAngles.z);
                        break;
                }
            }
        }
        public void RecordTank()
        {
            for(int i = 0;i < AllPlayers.Length; i++)
            {
                RecordTransform(AllPlayers[i].transform, TransformRecordMode.All);
                RecordTransform(AllPlayers[i].motion.tankComponents.turret.transform, TransformRecordMode.OnlyRotationY, true);
                RecordTransform(AllPlayers[i].motion.tankComponents.gun.transform, TransformRecordMode.OnlyRotationX, true);
                WriteSingleInt(AllPlayerStats[i].health);
            }
        }        
    }
}
