using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ZF.MainGame
{
    public class MainGameUI : MonoBehaviour
    {
        public Text TextFPS;
        public Text TextPing;

        public Text TextHealth;
        public Text TextTotalOutput;
        public Text TextHit;
        public Text TextKill;
        public Text TextSpeed;
        public Text[] ShellRemainTexts;


        private Stats.TankStat stat;
        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            ControlCursor();
            UpdateFPSText();
            UpdateSpeedText();
        }
        private void OnGUI()
        {

        }

        public void Init(Stats.TankStat _stat)
        {
            enabled = true;
            stat = _stat;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lastTime = Time.time;
        }

        private float lastTime;
        private void UpdateFPSText()
        {
            if(Time.frameCount % 40 == 0)
            {
                TextFPS.text = "FPS:" + (40f/ (Time.time - lastTime)).ToString("f2");
                lastTime = Time.time;
            }
        }
        public void UpdateGameStatTexts()
        {
            TextHealth.text = "生命值:" + stat.health.ToString();
            TextTotalOutput.text = "输出:" + stat.output.ToString();
            TextHit.text = "击中:" + stat.hit.ToString();
            TextKill.text = "击毁:" + stat.kill.ToString();
            for(int i =0;i < stat.AmmoRemain.Length;i++)
            {
                ShellRemainTexts[i].text = stat.AmmoRemain[i].ToString();
            }
        }
        public void UpdateSpeedText()
        {
            TextSpeed.text = "速度:" + stat.speed.ToString();
        }


        private bool cursorLocked = true;
        private void ControlCursor()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                if(!cursorLocked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    cursorLocked = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    cursorLocked = false;
                }
            }
        }
    }
}
