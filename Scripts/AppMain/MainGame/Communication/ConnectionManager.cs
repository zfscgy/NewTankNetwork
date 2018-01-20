using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZF.Communication
{
    public class ConnectionManager:Photon.MonoBehaviour
    {
        public InstructionManager instructionManager;
        private Instruction currentInstruction;

        private void Start()
        {
            if(photonView.isMine)
            {
                currentInstruction = instructionManager.GetInstruction();
            }
        }
        private void Update()
        {
            
        }
        private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(currentInstruction.ToByte());
            }
            else
            {
                currentInstruction = Instruction.ByteToInstruction((byte[])stream.ReceiveNext());
            }
        }

        public Instruction GetInstruction()
        {
            return currentInstruction;
        }
    }
}
