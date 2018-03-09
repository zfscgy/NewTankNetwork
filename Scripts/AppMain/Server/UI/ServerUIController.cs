using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace ZF.Server
{
    public class ServerUIController : MonoBehaviour
    {
        public Button button_TerminateGame;
        public GameObject Panel_Menu;
        public AI.MainGameAIController mainGameAIController;
        private bool cursorLocked = true;
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Panel_Menu.SetActive(!Panel_Menu.activeSelf);
                if (Panel_Menu.activeSelf)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    cursorLocked = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    cursorLocked = true;
                }
            }
        }


        public void Init()
        {
            gameObject.SetActive(true);
        }


        public void OnClick_TerminateGame()
        {
            if (Global.GameState.mode == Global.GameMode.inOfflineGame)
            {
                SceneManager.LoadScene("GameStart");
            }
            else if (Global.GameState.mode == Global.GameMode.isServer)
            {
                PhotonNetwork.LoadLevel("GameStart");
            }
        }
        public void OnClick_AddBot()
        {
            mainGameAIController.AddOneBot();
        }
    }
}
