using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum GameMode
{
    OnLine,
    OffLine,
}


public class WholeGameController : MonoBehaviour {
    /*
     * Control the whole game
     */
    private GameMode mode;
	void Start () {
		
	}
	
	void Update () {
		
	}



    void OnMainGameLoaded()
    {
        if(mode == GameMode.OffLine)
        {
            //Instantiate();

        }
    }
}
