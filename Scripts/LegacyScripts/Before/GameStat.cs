using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameStat : MonoBehaviour
{
    public Transform[] Panels;
	// Use this for initialization
	void Start ()
    {
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            ExitGames.Client.Photon.Hashtable playerProperty = player.CustomProperties;
            int id = (int)playerProperty["index"];
            int[] stat = (int[])playerProperty["GameStat"];
            Panels[id].Find("NameText").GetComponent<Text>().text = player.NickName;
            Panels[id].Find("ScoreText").GetComponent<Text>().text = stat[1].ToString();
            Panels[id].Find("HitText").GetComponent<Text>().text = stat[0].ToString();
            Panels[id].Find("KillText").GetComponent<Text>().text = stat[2].ToString();
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Button Event
    public void BackToRoom()
    {
        GameObject.Find("DNTD_WholeGameManager").GetComponent<WholeGameManager>().BackToRoom();
        SceneManager.LoadScene("Waiting");
    }
}
