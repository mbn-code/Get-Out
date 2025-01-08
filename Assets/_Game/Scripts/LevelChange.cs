using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LRS
{
    public class LevelChange : MonoBehaviour
    {
        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.name == "Player")
            {
                SceneManager.LoadScene("Underground Scene 3");
            }
        }
    }
}
