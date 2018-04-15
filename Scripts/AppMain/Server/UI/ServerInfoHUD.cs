using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
namespace ZF.Server
{
    using ZF.MainGame.Base;
    using ZF.MainGame.Stats;
    public class ServerInfoHUD : MonoBehaviour
    {
        public Text[] PlayerInfoTexts;
        private TankStat[] PlayerInfoStats;
        private Tank[] PlayerTanks;
        private Camera camera;
        public void Init(TankStat[] _PlayerInfoStats, Tank[] _PlayerTanks, Camera _camera)
        {
            PlayerInfoStats = _PlayerInfoStats;
            PlayerTanks = _PlayerTanks;
            camera = _camera;
            for(int i = 0; i < PlayerTanks.Length; i++)
            {
                if (i < 5)
                {
                    PlayerInfoTexts[i].color = Color.red;
                }
                else
                {
                    PlayerInfoTexts[i].color = Color.blue;
                }
            }
        }

        private void Update()
        {
            for(int i = 0; i < PlayerTanks.Length; i++)
            {
                if (PlayerTanks[i] != null)
                {
                    PlayerInfoTexts[i].rectTransform.position =
                        camera.WorldToScreenPoint(PlayerTanks[i].transform.position + new Vector3(0,5,0)) + new Vector3(80, -30);
                    if (PlayerInfoTexts[i].rectTransform.position.z > 0)
                    {
                        PlayerInfoTexts[i].text = i.ToString() + ":" + PlayerInfoStats[i].health.ToString();
                    }
                    else
                    {
                        PlayerInfoTexts[i].text = "";
                    }
                }
            }
        }

    }
}
