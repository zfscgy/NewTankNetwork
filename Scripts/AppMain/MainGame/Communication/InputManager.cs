using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZF.Communication
{
    /*  0 for not getting key, 1 for getting key
    *  KeyAction    |          |         |              |            |D|A|S|W|
    *  mouseAction  | | |RightDown|LeftDown|Right|Left|ScrollBackward|ScrollFoward|
    *  ...etc.
    */
    public class InputManager :MonoBehaviour
    {
        private byte KeyAction;
        private byte MouseAction;
        private float mouseX;
        private float mouseY;
        private void Update()
        {
            KeyAction = 0;
            MouseAction = 0;
            if (Input.GetKey(KeyCode.W))
            {
                KeyAction += (1 << 0);
            }
            if (Input.GetKey(KeyCode.S))
            {
                KeyAction += (1 << 1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                KeyAction += (1 << 2);
            }
            if (Input.GetKey(KeyCode.D))
            {
                KeyAction += (1 << 3);
            }
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            MouseAction += (Input.GetAxis("Mouse ScrollWheel") > 0) ? (byte)1 : (byte)0;
            MouseAction += (Input.GetAxis("Mouse ScrollWheel") < 0) ? (byte)2 : (byte)0;
            if(Input.GetMouseButton(0))
            {
                MouseAction += (1 << 2);
            }
            if(Input.GetMouseButton(1))
            {
                MouseAction += (1 << 3);
            }
            if(Input.GetMouseButtonDown(0))
            {
                MouseAction += (1 << 4);
            }
            if(Input.GetMouseButtonDown(1))
            {
                MouseAction += (1 << 5);
            }
        }


        // -1:S  0: 1:W
        public sbyte GetWS()
        {
            if (KeyAction % 2 == 1 && (KeyAction >> 1) % 2 == 0) 
            {
                return 1;
            }
            if (KeyAction % 2 == 0 && (KeyAction >> 1) % 2 == 1)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        // -1:D  0:  1:A
        public sbyte GetAD()
        {
            if ((KeyAction >> 2) % 2 == 1 && (KeyAction >> 3) % 2 == 0)
            {
                return 1;
            }
            if ((KeyAction >> 2) % 2 == 0 && (KeyAction >> 3) % 2 == 1)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public sbyte GetScroll()
        {
            if ((MouseAction >> 0) % 2 == 1 && (MouseAction >> 1) % 2 == 0)
            {
                return 1;
            }
            if ((MouseAction >> 0) % 2 == 0 && (MouseAction >> 1) % 2 == 1)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public bool GetMouseLeft()
        {
            return (MouseAction >> 2) % 2 == 1;
        }
        public bool GetMouseRight()
        {
            return (MouseAction >> 3) % 2 == 1; 
        }
        public bool GetMouseLeftDown()
        {
            return (MouseAction >> 4) % 2 == 1;
        }
        public bool GetMouseRightDown()
        {
            return (MouseAction >> 5) % 2 == 1;
        }
        public Vector2 GetMouseMotion()
        {
            return new Vector2(mouseX, mouseY);
        }

        public byte GetKeyAction()
        {
            return KeyAction;
        }
        public byte GetMouseAction()
        {
            return MouseAction;
        }
    }
}
