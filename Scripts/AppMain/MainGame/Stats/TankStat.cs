using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.MainGame.Stats
{
    using Global;
    public class TankStatParseException : Exception
    {
        public TankStatParseException()
        { }

        public TankStatParseException(string message)
        : base(message)
        { }

        public TankStatParseException(string message, Exception innerException)
        : base(message, innerException)
        { }
    }

    public class TankStat
    {
        public int health;
        public int output;
        public int kill;
        public int hit;
        public int speed;
        public int[] AmmoRemain = new int[GameSettings.n_AmmoKind];
        public int[] PartHealth = new int[GameSettings.n_TankBodyPart];

        public byte[] ToByte5()
        {
            byte[] Byte5 = new byte[5];
            Byte5[0] = (byte)health;
            Byte5[1] = (byte)(output / 256);
            Byte5[2] = (byte)(output % 256);
            Byte5[3] = (byte)kill;
            Byte5[4] = (byte)speed;
            return Byte5;
        }
        public byte[] ToBytesAll()
        {
            byte[] BytesAll = new byte[6 + AmmoRemain.Length + PartHealth.Length];
            BytesAll[0] = (byte)health;
            BytesAll[1] = (byte)(output / 256);
            BytesAll[2] = (byte)(output % 256);
            BytesAll[3] = (byte)kill;
            BytesAll[4] = (byte)hit;
            BytesAll[5] = (byte)speed;
            int i = 0;
            for (; i < AmmoRemain.Length; i++)
            {
                BytesAll[6 + i] = (byte)AmmoRemain[i];
            }
            for (i = 0; i < PartHealth.Length; i++)
            {
                BytesAll[6 + AmmoRemain.Length + i] = (byte)PartHealth[i];
            }
            return BytesAll;            
        }
        public void SetByBytes5(byte[] Bytes5, int index = 0)
        {
            health = (int)Bytes5[index + 0];
            output = (int)Bytes5[index + 1] * 256 + (int)Bytes5[index + 2];
            kill = (int)Bytes5[index + 3];
            speed = (int)Bytes5[index + 4];
        }
        public void SetByBytesAll(byte[] BytesAll, int index = 0)
        {
            health = (int)BytesAll[index + 0];
            output = (int)BytesAll[index + 1] * 256 + (int)BytesAll[index + 2];
            kill = (int)BytesAll[index + 3];
            hit = (int)BytesAll[index + 4];
            speed = (int)BytesAll[index + 5];
            int i;
            for (i = 0; i < GameSettings.n_AmmoKind; i++)
            {
                AmmoRemain[i] = (int)BytesAll[6 + i];
            }
            for (i = 0; i < GameSettings.n_TankBodyPart; i++)
            {
               PartHealth[i] = (int)BytesAll[6 + GameSettings.n_AmmoKind + i];
            }
        }
        public static TankStat Bytes5ToTankStat(byte[] Bytes5)
        {
            if(Bytes5.Length != 5)
            {
                throw new TankStatParseException("Input Byte4 length error");
            }
            TankStat tankStat = new TankStat
            {
                health = (int)Bytes5[0],
                output = (int)Bytes5[1] * 256 + (int)Bytes5[2],
                kill = (int)Bytes5[3],
                speed = (int)Bytes5[4]
            };
            return tankStat;
        }
        public static TankStat BytesAllToTankStat(byte[] BytesAll)
        {
            if (BytesAll.Length != 5 + GameSettings.n_AmmoKind + GameSettings.n_TankBodyPart)
            {
                throw new TankStatParseException("Input BytesAll length error");
            }
            TankStat tankStat = new TankStat
            {
                health = (int)BytesAll[0],
                output = (int)BytesAll[1] * 256 + (int)BytesAll[2],
                kill = (int)BytesAll[3],
                speed = (int)BytesAll[4],
                hit = (int)BytesAll[5],
            };
            int i;
            for( i = 0; i< GameSettings.n_AmmoKind; i++)
            {
                tankStat.AmmoRemain[i] = (int)BytesAll[6 + i];
            }
            tankStat.PartHealth = new int[GameSettings.n_TankBodyPart];
            for(i = 0;i<GameSettings.n_TankBodyPart;i++)
            {
                tankStat.PartHealth[i] = (int)BytesAll[6 + GameSettings.n_AmmoKind + i];
            }
            return tankStat;
        }
    }
}
