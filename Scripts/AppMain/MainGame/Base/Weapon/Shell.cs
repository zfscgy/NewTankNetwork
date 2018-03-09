using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.MainGame.Base.Weapon
{
    public class Shell : Photon.MonoBehaviour
    {
        public GameObject[] Explosions;
        public int damage;

        public  TankWeapon shooter;
        private int maxUpdateTime = 0;
        private int currentUpdate = 0;
        private Vector3 speed;
        private Vector3 explosionPosition;
        private bool isOwner = false;
        private bool isOnGameServer = false;
        private bool dead = false;

        public bool hit = false;
        private void FixedUpdate()
        {
            if (!isOwner)
            {
                return;
            }
            if(dead)
            {
                if (isOnGameServer)
                {
                    PhotonNetwork.Destroy(photonView);
                }
                else if(isOwner)
                {
                    Destroy(gameObject);
                }
            }
            currentUpdate++;
            if( currentUpdate > maxUpdateTime)
            {
                DestroySelf();
            }

        }


        public void Init(TankWeapon _shooter, int _damage, Vector3 _speed, float remainTime, bool _isOnline)
        {
            shooter = _shooter;
            damage = _damage;
            GetComponent<Rigidbody>().isKinematic = false;
            speed = _speed;
            maxUpdateTime = (int)(remainTime / Time.fixedDeltaTime);
            GetComponent<Rigidbody>().velocity = speed;
            isOwner = true;
            isOnGameServer = _isOnline;
        }

        public void DestroySelf()
        {
            dead = true;
        }

        public int GetUpdateNumber()
        {
            return  currentUpdate;
        }

        public void HitCollier(HitMode hitMode, int damage, bool isKilled)
        {
            if (hitMode == HitMode.Tank)
            {
                hit = true;
                shooter.HitResult(damage, isKilled);
            }
            if (isOnGameServer)
            {
                photonView.RPC("RPCDetonate", PhotonTargets.All, explosionPosition, hitMode);
            }
            else
            {
                RPCDetonate(explosionPosition, hitMode);
            }
        }

        [PunRPC]
        public void RPCDetonate(Vector3 _explosionPosition, HitMode hitMode)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponentInChildren<Collider>().enabled = false;
            Instantiate(Explosions[(int)hitMode], _explosionPosition, transform.rotation);
            gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        public void SetExplosionPosition(Vector3 position)
        {
            explosionPosition = position;
        }
    }
}
