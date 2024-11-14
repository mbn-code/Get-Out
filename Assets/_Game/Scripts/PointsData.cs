/*
 * Author: Leonhard Robin Schnaitl
 * GitHub: https://github.com/leonhardrobin
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace LRS
{
    [CreateAssetMenu(fileName = "New PointsData", menuName = "Points Data")]
    public class PointsData : ScriptableObject
    {
        public List<string> includedTags = new();
        public VisualEffect prefab;
        [HideInInspector] public VisualEffect currentVisualEffect;
        [HideInInspector] public List<Vector3> positionsList = new();
        [HideInInspector] public Texture2D texture;
        public List<float> timestamps = new(); // Add this line
        [HideInInspector] public Color[] positionsAsColors;

        [ContextMenu("Clear Data")]
        public void ClearData()
        {
            currentVisualEffect = null;
            positionsList.Clear();
            texture = null;
            positionsAsColors = null;
            timestamps.Clear();
        }
    }
}

