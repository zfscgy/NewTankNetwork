using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarControl : Photon.MonoBehaviour {
    //local player transform
    public Transform radar;
    public Dictionary<int, TankControl> tankDict = new Dictionary<int, TankControl>();

    public List<Transform> TrackedObjects = new List<Transform>();
    public List<Transform> FriendObjects = new List<Transform>();
    public RectTransform minimap;
    private ExitGames.Client.Photon.Hashtable radarInfo;
    private byte[] followedList = new byte[2 * Global.MAX_NUMBER_EACH_SIDE] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public float maxSensorRange = 200f;
    public float maxBombSensorRange = 300f;
    public GameObject bombPoint;

    // Use this for initialization
    void Start()
    {
        radarInfo = new ExitGames.Client.Photon.Hashtable();
        radarInfo.Add("followedList",followedList);
        PhotonNetwork.player.SetCustomProperties(radarInfo);
    }
    // Update is called once per frame
    void Update()
    {
        if ((Time.frameCount % 32) == 0)
        {
            byte[] fList = (byte[])radarInfo["followedList"];
            foreach(Transform enemy in TrackedObjects)
            {
        //        enemy.GetComponent<TankControl>().radarPoint.SetActive(false);
                if(enemy == null)
                {
                    continue;
                }
                
                //Debug.Log("EnemyDistance:" + (enemy.position - radar.position).magnitude.ToString());
                if ((enemy.position - radar.position).magnitude > maxSensorRange)
                {
                    enemy.GetComponent<TankControl>().radarPoint.SetActive(false);
                    fList[enemy.gameObject.GetPhotonView().ownerId] = 0;
                }
                else
                {
                    enemy.GetComponent<TankControl>().radarPoint.SetActive(true);
                    //Sync on the network
                    fList[enemy.gameObject.GetPhotonView().ownerId] = 1;
                }

            }
            ExitGames.Client.Photon.Hashtable hashable = new ExitGames.Client.Photon.Hashtable();
            hashable.Add("followedList", fList);
            PhotonNetwork.player.SetCustomProperties(hashable);
            Debug.Log("Sync Radar");

            foreach(Transform friendTank in FriendObjects)
            {
                if(friendTank == null)
                {
                    continue;
                }
                byte[] friendList = (byte[])friendTank.gameObject.GetPhotonView().owner.CustomProperties["followedList"];
                for (int i = 1; i <= 2 * Global.MAX_NUMBER_EACH_SIDE; i++)
                {                   
                    if (friendList[i - 1] != 0)
                    {
                        tankDict[i - 1].radarPoint.SetActive(true);
                    }
                }
            }
        }

    }

    #region Public Methods
    //Check if the explosion is within sensor range, if so, then create a tag on the minimap
    public bool CreateBombTag(Vector3 bombPosition, Quaternion bombRotation,ZF.Flag flag)
    {
        if ((bombPosition - radar.position).magnitude < maxBombSensorRange)
        {
            //GameObject bombTag = Instantiate(bombPoint, bombPosition + new Vector3(0, 30, 0), bombRotation);
            //bombTag.GetComponentInChildren<SpriteRenderer>().color = (flag == ZF.Flag.Blue) ? Color.blue : Color.red;
            photonView.RPC("RPCCreateBombTag", PhotonTargets.All, bombPosition, bombRotation, flag);
            return true;
        }
        return false;
    }
    [PunRPC]
    public bool RPCCreateBombTag(Vector3 bombPosition, Quaternion bombRotation, ZF.Flag flag)
    {
        Debug.Log("RPCCreateBombTag Called");
        if(flag == radar.GetComponent<TankControl>().flag)
        {
            GameObject bombTag = Instantiate(bombPoint, bombPosition + new Vector3(0, 30, 0), bombRotation);
            bombTag.GetComponentInChildren<SpriteRenderer>().color = (flag == ZF.Flag.Blue) ? Color.blue : Color.red;
            return true;
        }
        return false;
    }
    #endregion

}
