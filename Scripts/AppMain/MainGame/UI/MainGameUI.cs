using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace ZF.MainGame
{
    public class MainGameUI : MonoBehaviour
    {
        public Text FPSText;
        // Use this for initialization
        private void Start()
        {
            lastTime = Time.time;
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateFPSText();
        }
        private void OnGUI()
        {

        }

        private float lastTime;
        private void UpdateFPSText()
        {
            if(Time.frameCount % 40 == 0)
            {
                FPSText.text = "FPS:" + (40f/ (Time.time - lastTime)).ToString("f2");
                lastTime = Time.time;
            }
        }
    }
}
