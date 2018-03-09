using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZF.StartGame
{
    public class GameStartPrepare : MonoBehaviour
    {
        private void Awake()
        {
            if (Global.Singletons.wholeGameController != null)
            {
                Global.Singletons.wholeGameController.LoadBackToStartScene();
            }
        }
        void Start()
        {

        }

        void Update()
        {

        }
    }
}
