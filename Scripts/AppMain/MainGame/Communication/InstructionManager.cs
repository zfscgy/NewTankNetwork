
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace ZF.Communication
{
    [System.Serializable]
    public class InstructionParseException : Exception
    {
        public InstructionParseException()
        { }

        public InstructionParseException(string message) 
        : base(message)
        { }

        public InstructionParseException(string message, Exception innerException)
        : base (message, innerException)
        { }
    }
    public class Instruction
    {
        private const int length =  14;
        private Vector3 targetPosition;
        private byte keyAction;
        private byte mouseAction;

        public Instruction()
        {
            keyAction = 0;
            mouseAction = 0;
            targetPosition = new Vector3(0,0,0);
        }
        /*
         * Convert Instruction to a size-2 byte-array
         */
        public byte[] ToByte()
        {
            byte[] bytes = new byte[length];
            bytes[0] = keyAction;
            bytes[1] = mouseAction;
            System.BitConverter.GetBytes(targetPosition.x).CopyTo(bytes,2);
            System.BitConverter.GetBytes(targetPosition.y).CopyTo(bytes, 6);
            System.BitConverter.GetBytes(targetPosition.z).CopyTo(bytes, 10);
            return bytes;
        }

        /*
        * Convert byte-array to Instruction
         */
        public void BytesToInstruction(byte[] InputBytes)
        {
            if (InputBytes.Length != length)
            {
                throw new InstructionParseException("Byte-Array's length is not correct.");
            }
            keyAction = InputBytes[0];
            mouseAction = InputBytes[1];
            targetPosition.x = BitConverter.ToSingle(InputBytes, 2);
            targetPosition.y = BitConverter.ToSingle(InputBytes, 6);
            targetPosition.z = BitConverter.ToSingle(InputBytes, 10);
        }

        public void SetInstruction(byte _keyAction,byte _mouseAction,Vector3 _targetPosition)
        {
            keyAction = _keyAction;
            mouseAction = _mouseAction;
            targetPosition = _targetPosition;
        }

        // -1:S  0: 1:W
        public sbyte GetWS()
        {
            if (keyAction % 2 == 1 && (keyAction >> 1) % 2 == 0)
            {
                return 1;
            }
            if (keyAction % 2 == 0 && (keyAction >> 1) % 2 == 1)
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
            if ((keyAction >> 2) % 2 == 1 && (keyAction >> 3) % 2 == 0)
            {
                return 1;
            }
            if ((keyAction >> 2) % 2 == 0 && (keyAction >> 3) % 2 == 1)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public Vector3 GetTargetPosition()
        {
            return targetPosition;
        }
    }
    public class InstructionManager: MonoBehaviour
    {
        public InputManager inputManager;
        public MainGame.Base.CameraController cameraController;
        private bool isOverride = false;
        private Instruction currentInstruction = new Instruction();
        #region Monobehavior Callbacks
        private void Start()
        {
            if (Global.GameState.mode == Global.GameMode.isServer)
            {
                enabled = false;
            }
        }
        private void Update()
        {
            if(isOverride)
            {
                return;
            }
            Vector3 targetPosition = cameraController.GetCameraPointing(); 
            currentInstruction.SetInstruction(inputManager.GetKeyAction(), inputManager.GetMouseAction(), targetPosition);
        }
        #endregion
        public Instruction GetInstruction()
        {
            return currentInstruction;
        }
        
        public void SetCamera(ZF.MainGame.Base.CameraController _cameraController)
        {
            cameraController = _cameraController;
        }

        public void SetOverride(bool _isOverride)
        {
            isOverride = _isOverride;
        }
        public void Override(Instruction overrideIns)
        {
            currentInstruction = overrideIns;
        }
    }
}