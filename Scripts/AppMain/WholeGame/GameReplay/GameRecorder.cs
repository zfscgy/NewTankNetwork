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
    using ZF.WholeGame;
    using Global;
    public class GameRecordException : Exception
    {
        public GameRecordException() { }
        public GameRecordException(string message)
        : base(message)
        { }
        public GameRecordException(string message, Exception innerException)
        : base(message, innerException)
        { }
    }
    /// <summary>
    ///  Recorder, recording all player tanks' states during the game
    /// </summary>
    public class GameRecorder:MonoBehaviour
    {
        private FileStream fstream;
        private BinaryWriter binaryWriter;
        private Tank[] AllPlayers;
        private GameInteractionManager interactionManager = Singletons.gameInteractionManager;
        private TankStat[] AllPlayerStats;
        public void Init(Tank[] _AllPlayers, TankStat[] _AllPlayerStats, string filename)
        {
            if (CreateFile(filename))
            {
                AllPlayers = _AllPlayers;
                AllPlayerStats = _AllPlayerStats;
                return;
            }

            throw new GameRecordException("Record file already exists!");
        }
        private bool CreateFile(string filename)
        {
            if (File.Exists(filename))
            {
                return false;
            }
            fstream = File.Open(filename, FileMode.Create);
            binaryWriter = new BinaryWriter(fstream);
            return true;
        }
        public void StartRecord()
        {
            new GameSaveFileHeader().Write(binaryWriter);
            enabled = true;
        }
        public void StopRecord()
        {
            binaryWriter.Close();
            enabled = false;
        }

        private void WriteSingleInt(int value)
        {
            binaryWriter.Write(value);
        }
        private void RecordTransform(Transform transform, TransformRecordMode mode, bool isLocal = false)
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
        private void RecordFrame()
        {
            if(binaryWriter == null)
            {
                return;
            }
            for (int i = 0; i < AllPlayers.Length; i++)
            {
                if (AllPlayers[i] != null)
                {
                    RecordTransform(AllPlayers[i].transform, TransformRecordMode.All);
                    RecordTransform(AllPlayers[i].motion.tankComponents.turret.transform, TransformRecordMode.OnlyRotationY, true);
                    RecordTransform(AllPlayers[i].motion.tankComponents.gun.transform, TransformRecordMode.OnlyRotationX, true);
                }
                else
                {
                    RecordTransform(transform, TransformRecordMode.All);
                    RecordTransform(transform, TransformRecordMode.OnlyRotationY, true);
                    RecordTransform(transform, TransformRecordMode.OnlyRotationX, true);
                }
                if (AllPlayerStats[i] != null)
                {
                    WriteSingleInt(AllPlayerStats[i].health);
                }
                else
                {
                    WriteSingleInt(0);
                }
            }
            WriteSingleInt(interactionManager.GetUnRecordedHits().Count);
            for(int i = 0; i < interactionManager.GetUnRecordedHits().Count; i++)
            {
                HitInfo hitInfo = interactionManager.GetUnRecordedHits()[i];
                WriteSingleInt(hitInfo.attackerSeat);
                WriteSingleInt(hitInfo.victimSeat);
                WriteSingleInt(hitInfo.totalDamage);
            }
        }


        private void Update()
        {
            RecordFrame();
        }
    }
}
