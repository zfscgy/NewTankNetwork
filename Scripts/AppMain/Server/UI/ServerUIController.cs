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
        public GameObject Panel_AddAI;

        public AI.MainGameAIController mainGameAIController;
        private bool cursorLocked = true;

        private bool isPanelActive = false;
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if (isPanelActive == false)
                {
                    Panel_Menu.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    cursorLocked = false;
                    isPanelActive = true;
                }
                else
                {
                    Panel_Menu.SetActive(false);
                    Panel_AddAI.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    cursorLocked = true;
                    isPanelActive = false;
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
            Panel_Menu.SetActive(false);
            for(int i = 0; i< Panel_AddAI.transform.childCount; i++)
            {
                if(Global.Singletons.gameRoutineController.GetTanks()[i] == null)
                {
                    Panel_AddAI.transform.GetChild(i).GetComponent<Button>().enabled = true;
                }
                else
                {
                    Panel_AddAI.transform.GetChild(i).GetComponent<Button>().enabled = false;
                }
            }
            Panel_AddAI.SetActive(true);
        }

        public void OnClick_AddBotAtIndex(int id)
        {
            mainGameAIController.AddOneBot(id);
        }

        public void OnClick_StartAllAI()
        {
            mainGameAIController.StartAllAI();
        }
    }
}
