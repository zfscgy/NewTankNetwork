using UnityEngine;
using ZF;

public class Bullet : Photon.MonoBehaviour
{
    //Bullet Parameters
    public float speed = 1000;
    private int damage;
    //Shooter's view ID
    private int shooterViewID;
    private Flag flag;

    //爆炸粒子效果
    public GameObject shooter;
    public GameObject explosion;
    public GameObject bigExplosion;
    //子弹的最大生存时间，当燃尽燃料，则摧毁炮弹
    public float maxExistTime;
    //记录炮弹发射时间，计算是否耗尽燃料
    public float instantiateTime = 0f;

    private byte isFirstCollision = 1;
    // Use this for initialization
    // The reason to use Awake Instead of Start is that Bullet moves very fast
    // And sometimes when we havn't executed Start function it will hit a collider and 
    // Invoke the OnCollisionEnter function. So it will have exceptions.
    // By replace Start with Awake, we can let the initialization go before physics functions,
    // So even the OnCollisionEnter happens in the exact time when it is created,
    // We don't need to worry, Awake executes immediately after the script is created.
    void Awake()
    {
        //Read data from "TankConfig.xml"
        //readit = new ReadXml();
        //readit.SetBullet("Assets\\Scripts\\TankConfig.xml", "Bullet", this);
        //ChooseTankBullet();
        if (!photonView.isMine)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        //记录炮弹发射时间
        instantiateTime = Time.time;
        GetComponent<Rigidbody>().velocity = speed * transform.forward + (Vector3)photonView.instantiationData[1];
        damage = (int)photonView.instantiationData[2];
        shooterViewID = (int)photonView.instantiationData[0];
        Debug.Log("Bullet Created with ShooterViewID:" + shooterViewID);
    }

    // Update is called once per frame
    void Update()
    {
        //判断是否炮弹燃料耗尽，是否摧毁炮弹
        if ((Time.time - instantiateTime) > maxExistTime)
        {
            Destroy(gameObject);
        }
    }

    //子弹碰撞时爆炸
    void OnCollisionEnter(Collision collisionInfo)
    {
        /*foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }*/
        if (isFirstCollision == 0)
        {
            return;
        }
        isFirstCollision = 0;
        Vector3 hitPos = collisionInfo.contacts[0].point;
        Debug.Log("Bullet Hit Point:" + hitPos);
        //Following Code will be executed in every client
        //Just to show the tag in radarPoint
        RadarControl radar = GameObject.Find("GameController").GetComponent<RadarControl>();
        radar.CreateBombTag(hitPos, gameObject.transform.rotation,PhotonView.Find(shooterViewID).GetComponent<TankControl>().flag);

        //Only executed in the owner of the bullet
        if (!photonView.isMine)
        {
            return;
        }
        //Notice: collisionInfo.gameObject will return the most upper level object,in this case,"tank1(Clone)"
        string hitPart = collisionInfo.collider.gameObject.name;
        TankBody hitBody;
        GameObject hitObj;
        if (collisionInfo.collider.transform.parent != null)
        {
            hitObj = collisionInfo.collider.transform.parent.gameObject;
        }
        else
        {
            hitObj = null;
        }
        
        //If have hit a tank
        if (hitObj!=null &&hitObj.tag == "Player")
        {
            switch (hitPart)
            {
                case "Bottom": hitBody = TankBody.Bottom; break;
                case "Turret": hitBody = TankBody.Turret; break;
                default: hitBody = TankBody.IsNotTankBody; break;
            }

            HitInfo shooterSideInfo = new HitInfo();
            shooterSideInfo.shooterDamage = (ushort)damage;
            shooterSideInfo.shooterViewID = (ushort)shooterViewID;
            shooterSideInfo.shooterNetworkID = (byte)photonView.ownerId;
            shooterSideInfo.hitPart = hitBody;
            //Make a RPC call to hitObject's owner
            hitObj.GetPhotonView().RPC("RPCTakeDamage", hitObj.GetPhotonView().owner, shooterSideInfo.ToBytes());
            PhotonNetwork.Instantiate(bigExplosion.name, hitPos, transform.rotation, 0);
            //DestroySelf
            PhotonNetwork.Destroy(gameObject);
            return;
        }
        //If just hit the ground
        //添加爆炸效果
        PhotonNetwork.Instantiate(explosion.name, hitPos, transform.rotation, 0);
        //摧毁自身
        PhotonNetwork.Destroy(gameObject);
    }

}
/*
 *
    private void ChooseTankBullet()
    {
        ExitGames.Client.Photon.Hashtable playerProperties = PhotonNetwork.player.AllProperties;
        //ordinaryTank 
        if ((int)playerProperties["TankModel"] == 0)
        {
            speed = 1000;
            explosion.name = "ExplosionMobile";
            maxExistTime = 10f;
            Debug.Log("TankControl.ChooseTankBullet.ordinaryTank0");
        }
        //fastTank
        if ((int)playerProperties["TankModel"] == 1)
        {
            speed = 50;
            explosion.name = "ExplosionMobile";
            maxExistTime = 2f;
            Debug.Log("TankControl.ChooseTankBullet.fastTank1");
        }
        //heavyTank
        if ((int)playerProperties["TankModel"] == 2)
        {
            speed = 200;
            explosion.name = "Explosion";
            maxExistTime = 10f;
            Debug.Log("TankControl.ChooseTankBullet.heavyTank2");
        }

    }
*/