using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.MainGame.Base
{
    public class TankArmor : MonoBehaviour
    {
        public TankBody body;
        public Collider[] ArmorColliders;
        private bool detectTrigger = true;
        private void Start()
        {
            if (Global.GameState.mode != Global.GameMode.isServer && 
                Global.GameState.mode != Global.GameMode.inOfflineGame)
            {
                detectTrigger = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!detectTrigger)
            {
                return;
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            {
                Debug.Log("Shell hit armor!");
                Weapon.Shell shell = other.transform.parent.GetComponent<Weapon.Shell>();
                if (shell.hit)
                {
                    return;
                }
                RaycastHit hit;
                Vector3 lastPosition = shell.transform.position;
                if (shell.GetUpdateNumber() == 0)
                {
                    lastPosition -= 20f * shell.transform.forward;
                }
                else if (shell.GetUpdateNumber() == 1)
                {
                    lastPosition -= 25f * shell.transform.forward;
                }
                else
                {
                    lastPosition -= 30f * shell.transform.forward;
                }
                Ray trajectory = new Ray(lastPosition, shell.transform.forward);
                Physics.Raycast(trajectory, out hit, 40f, LayerMask.GetMask("Tank"));
                Collider hitCollider = hit.collider;
                shell.SetExplosionPosition(hit.point);
                int damage = 0;
                bool isKilled = false;
                TankPart part = 0;
                if (hitCollider.transform.parent.CompareTag("TankWheel"))
                {
                    damage = body.Hit(TankPart.Wheel, shell.damage, out isKilled);
                    part = TankPart.Wheel;
                }
                else
                {
                    for (int i = 0; i < ArmorColliders.Length; i++)
                    {
                        if (hitCollider == ArmorColliders[i])
                        {
                            part = (TankPart)i;
                            damage = body.Hit((TankPart)i, shell.damage, out isKilled);
                            break;
                        }
                    }
                }
                Stats.HitInfo hitInfo = new Stats.HitInfo();
                hitInfo.hitPart = part;
                hitInfo.totalDamage = (byte)damage;
                hitInfo.victimSeat = (byte) GetComponent<Tank>().seatID;
                hitInfo.attackerSeat = (byte)shell.shooter.GetComponent<Tank>().seatID;
                Global.GameState.hitStat.NewHit(hitInfo);
                shell.HitCollier(Weapon.HitMode.Tank, damage, isKilled);
            }
        }
    }
}
