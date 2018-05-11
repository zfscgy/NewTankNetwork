using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZF.MainGame.Base.Texts
{
    public class Info3dText : MonoBehaviour
    {
        public Transform followingTransform;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Camera.main!=null)
            {
                transform.position = followingTransform.position + new Vector3(0f, 5f, 0f);
                transform.LookAt(Camera.main.transform.position);
            }
        }
        public void Init(Color color,string str)
        {
            GetComponentInChildren<Renderer>().material.color = color;
            GetComponentInChildren<TextMesh>().text = str;
            enabled = true;
        }
    }
}
