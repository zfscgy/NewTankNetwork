using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.MainGame.Base.Weapon.Effect
{
    public class SelfDestroyer : MonoBehaviour
    {
        public float duration = 2.0f;
        private float startTime;

        void Start()
        {
            startTime = Time.time;
        }
        void Update()
        {
            if(Time.time - startTime > duration)
            {
                Destroy(gameObject);
            }
        }
    }
}
