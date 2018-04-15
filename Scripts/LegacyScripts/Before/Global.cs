using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Global
{
    public const int MAX_NUMBER_EACH_SIDE = 5;
}
public static class GameDataContainer
{
    public static int[] IndexTable = new int[2 * Global.MAX_NUMBER_EACH_SIDE]
        {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
}
