using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPicker : MonoBehaviour {
    private int[] IndexTable = new int[2*Global.MAX_NUMBER_EACH_SIDE]
        { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
    private bool CheckIndexLeagal(int uncheckedIndex)
    {
        return uncheckedIndex >= 0 && uncheckedIndex < 2 * Global.MAX_NUMBER_EACH_SIDE;
    }

    #region MonoBehaveior Callbacks
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    #endregion

    public void Reset()
    {
        for(int i =0;i<IndexTable.Length;i++)
        {
            IndexTable[i] = -1;
        }
    }
    public int AllocateNewIndex(int actorID, int startIndex)
    {
        if(!CheckIndexLeagal(startIndex))
        {
            return -1;
        }
        int counter = 0;
        int newAllocatedIndex = -1;
        for (int i = (int)startIndex; counter < 2 * Global.MAX_NUMBER_EACH_SIDE; i = (i + 1) % (2 * Global.MAX_NUMBER_EACH_SIDE))
        {
            counter++;
            if (IndexTable[i] == -1)
            {
                IndexTable[i] = (sbyte)actorID;
                newAllocatedIndex = i;
                break;
            }
        }
        return newAllocatedIndex;
    }
    public int SwitchIndex(int actorID)
    {
        int currentIndex = -1;
        for (int i = 0; i < IndexTable.Length; i++)
        {
            if(IndexTable[i] == (sbyte)actorID)
            {
                currentIndex = i;
                IndexTable[i] = -1;
                break;
            }
        }
        int startIndex = currentIndex >= Global.MAX_NUMBER_EACH_SIDE ? 0 : Global.MAX_NUMBER_EACH_SIDE;
        if (!CheckIndexLeagal(startIndex))
        {
            return -1;
        }
        IndexTable[currentIndex] = -1;
        return AllocateNewIndex(actorID, startIndex);
    }
    public int ResetIndex(int actorID)
    {
        for (int i = 0; i < IndexTable.Length; i++)
        {
            if (IndexTable[i] == (sbyte)actorID)
            {
                IndexTable[i] = -1;
                return -1;
            }
        }
        return actorID;
    }
}
