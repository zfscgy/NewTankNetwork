using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.MainGame.Base
{
    public class Ground : MonoBehaviour
    {
        private bool detectingTrigger = true;
        void Start()
        {
            Debug.Log(sizeof(float));
            if (Global.GameState.mode != Global.GameMode.isServer && 
                Global.GameState.mode != Global.GameMode.inOfflineGame)
            {
                detectingTrigger = false;
            }

        }

        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if(!detectingTrigger)
            {
                return;
            }
            GameObject collider = other.gameObject;
            if (collider.layer == LayerMask.NameToLayer("Bullet"))
            {
                Weapon.Shell shell = collider.transform.parent.GetComponent<Weapon.Shell>();
                Debug.Log("Shell hit ground!");
                Vector3 lastPosition;
                if (shell.GetUpdateNumber() == 0)
                {
                    lastPosition = shell.transform.position - 20f * shell.transform.forward;
                }
                else if (shell.GetUpdateNumber() == 1)
                {
                    lastPosition = shell.transform.position - 25f * shell.transform.forward;
                }
                else
                {
                    lastPosition = shell.transform.position - 30f * shell.transform.forward;
                }
                Ray trajectory = new Ray(lastPosition, shell.transform.forward);
                RaycastHit hit;
                Physics.Raycast(trajectory, out hit, 30f, LayerMask.GetMask("Ground"));
                shell.SetExplosionPosition(hit.point);
                shell.HitCollier(Weapon.HitMode.Ground, 0, false);
            }
        }
    }
}
