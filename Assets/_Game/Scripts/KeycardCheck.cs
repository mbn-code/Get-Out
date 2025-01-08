using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LRS
{
    public class KeycardCheck : MonoBehaviour
    {
        public GameObject Door;

        void OnCollisionEnter(Collision col)
        {
            Debug.Log("Collision");
            Debug.Log(col.gameObject.name);
            if (col.gameObject.name == "KeycardCheck")
            {
                Destroy(Door);
            }
        }
    }
}
