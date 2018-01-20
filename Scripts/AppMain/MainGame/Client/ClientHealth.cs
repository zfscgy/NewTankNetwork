using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.MainGame.Client
{
    using ZF.Configs;
    class ClientHealth:MonoBehaviour
    {
        public HealthConfig healthConfig;
        private float currentHealth;
        #region Monobehavior Callbacks
        private void Start()
        {
            currentHealth = healthConfig.totalHealth;
        }
        #endregion

        public float TakeDamage(float damage)
        {
            if(currentHealth < damage)
            {
                currentHealth -= damage;
            }
            else
            {
                damage = currentHealth;
                currentHealth = 0;
            }
            return damage;
        }

    }
}
