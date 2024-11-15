using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEditor.PlayerSettings;

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
        public float? spawnTimestamp;
        [HideInInspector] public Color[] positionsAsColors;

        [ContextMenu("Clear Data")]
        public void ClearData()
        {
            currentVisualEffect = null;
            positionsList.Clear();
            texture = null;
            positionsAsColors = null;
            spawnTimestamp = null;
        }

        public float GetAlpha(float currentTime, float pointLifetime)
        {
            float elapsedTime = (float)(currentTime - spawnTimestamp);
            return Mathf.Clamp01(1 - (elapsedTime / pointLifetime));
        }

        public IEnumerator AlphaLoop(float pointLifetime)
        {
            while (true)
            {
                Vector3[] pos = positionsList.ToArray();
                float currentTime = Time.time;
                Vector3 vfxPos = currentVisualEffect.transform.position;
                int loopLength = texture.width * texture.height;
                int posListLen = pos.Length;

                for (int i = 0; i < loopLength; i++)
                {
                    Color data;

                    if (i < posListLen - 1)
                    {
                        float alpha = GetAlpha(currentTime, pointLifetime); // Assuming all pointsData have the same lifetime

                        if (alpha <= 0)
                        {
                            break;
                        }

                        data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, alpha);
                    }
                    else
                    {
                        data = new Color(0, 0, 0, 0);
                    }

                    positionsAsColors[i] = data;
                }

                texture.SetPixels(positionsAsColors);
                texture.Apply();
                currentVisualEffect.SetTexture("PositionsTexture", texture);
                currentVisualEffect.Reinit();

                yield return new WaitForSeconds(1f / pointLifetime);
            }
        }

    }
}