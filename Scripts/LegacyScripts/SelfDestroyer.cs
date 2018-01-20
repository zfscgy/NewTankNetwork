using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.Particle
{
    public class SelfDestroyer : Photon.MonoBehaviour
    {
        public float duration = 2.0f;
        public bool isNetworkObject;
        private float startTime;
       
        // Use this for initialization
        void Start()
        {
            startTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if(Time.time - startTime > duration)
            {
                if (isNetworkObject&&photonView.isMine)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
