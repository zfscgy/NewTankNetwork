using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZF.WholeGame.GameReplay
{
    public enum TransformRecordMode
    {
        All,
        OnlyPosition,
        OnlyRotation,
        OnlyRotationX,
        OnlyRotationY,
        OnlyRotationZ,
    }
    // Header of the game's record file
    public class GameSaveFileHeader
    {
        //Just a message about the game
        public char[] header = "TankNetwork v1.1 Developed by Zheng Fei,USTC. \n".ToCharArray();
        public char[] readHeader;
        public int[] PlayerSeats = new int[Global.Global.playerPerRoom];
        public void Write(BinaryWriter writer)
        {
            writer.Write(header);
            for(int i = 0; i < PlayerSeats.Length; i++)
            {
                writer.Write(PlayerSeats[i]);
            }
        }
        public void Read(BinaryReader reader)
        {
            readHeader = new char[header.Length];
            reader.Read(readHeader, 0, header.Length);
            for(int i = 0;i < PlayerSeats.Length; i++)
            {
                PlayerSeats[i] = reader.ReadInt32();
            }
        }
    }
}
