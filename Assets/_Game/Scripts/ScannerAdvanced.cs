using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace LRS
{
    public class ScannerAdvanced : MonoBehaviour
    {
        [SerializeField] private List<PointsData> pointsData = new();

        private const string REJECT_LAYER_NAME = "PointReject";
        private const string TEXTURE_NAME = "PositionsTexture";
        private const string RESOLUTION_PARAMETER_NAME = "Resolution";

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private InputActionProperty pinchAction;
        private InputAction _fire => pinchAction.action;
        private GameObject vfxContainer;
        [SerializeField] private Transform castPoint;
        [SerializeField] private float radius = 10f;
        [SerializeField] private int pointsPerScan = 100;
        [SerializeField] private float range = 10f;
        [SerializeField] private int resolution = 100;
        [SerializeField] private float pointLifetime = 5f; // Duration in seconds
        [SerializeField] private float cooldownTime = 6f; // Cooldown duration in seconds
        [SerializeField] private float coneAngle = 30f; // Angle of the cone in degrees

        private bool _isCooldown;
        private float _lastFireTime;
        private bool _createNewVFX;

        private void Start()
        {
            vfxContainer = GameObject.Find("Graphs");

            pointsData.ForEach(data =>
            {
                data.ClearData();
                _createNewVFX = true;
                data.currentVisualEffect = NewVisualEffect(data.prefab, out data.texture, out data.positionsAsColors);
                ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
            });
        }

        private void FixedUpdate()
        {
            Scan();
            RemoveOldPoints();
        }

        private void ApplyPositions(List<Vector3> positionsList, VisualEffect currentVFX, Texture2D texture, Color[] positions)
        {
            Vector3[] pos = positionsList.ToArray();
            Vector3 vfxPos = currentVFX.transform.position;
            Vector3 transformPos = transform.position;
            int loopLength = texture.width * texture.height;
            int posListLen = pos.Length;
            float currentTime = Time.time;

            for (int i = 0; i < loopLength; i++)
            {
                Color data;

                if (i < posListLen - 1)
                {
                    data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1);
                }
                else
                {
                    data = new Color(0, 0, 0, 0);
                }
                positions[i] = data;
            }

            texture.SetPixels(positions);
            texture.Apply();
            currentVFX.SetTexture(TEXTURE_NAME, texture);
            currentVFX.Reinit();
        }

        private VisualEffect NewVisualEffect(VisualEffect visualEffect, out Texture2D texture, out Color[] positions)
        {
            if (!_createNewVFX)
            {
                texture = null;
                positions = new Color[] { };
                return null;
            }

            VisualEffect vfx = Instantiate(visualEffect, transform.position, Quaternion.identity, vfxContainer.transform);
            vfx.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution);
            texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            positions = new Color[resolution * resolution];

            _createNewVFX = false;

            return vfx;
        }

        private IEnumerator CoolDown()
        {
            yield return new WaitForSeconds(cooldownTime);
            _isCooldown = false;
        }

        private void Scan()
        {
            if (_fire.IsPressed() && !_isCooldown)
            {
                _isCooldown = true;
                StartCoroutine(CoolDown());
                for (int i = 0; i < pointsPerScan; i++)
                {
                    // Generate a random direction within the cone angle
                    float angleX = Random.Range(-coneAngle / 2, coneAngle / 2);
                    float angleY = Random.Range(-coneAngle / 2, coneAngle / 2);
                    Quaternion rotation = Quaternion.Euler(angleX, angleY, 0);
                    Vector3 randomDirection = rotation * castPoint.forward;

                    if (Physics.Raycast(castPoint.position, randomDirection, out RaycastHit hit, range, layerMask))
                    {
                        if (hit.collider.CompareTag(REJECT_LAYER_NAME)) continue;

                        pointsData.ForEach(data =>
                        {
                            data.spawnTimestamp = Time.time; // Set spawn time
                            data.includedTags.ForEach(tag =>
                            {
                                if (hit.collider.CompareTag(tag))
                                {
                                    data.positionsList.Add(hit.point);
                                }
                            });
                        });
                    }
                    else
                    {
                        Debug.DrawRay(castPoint.position, randomDirection * range, Color.red);
                    }
                }

                pointsData.ForEach(data =>
                {
                    ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
                    StartCoroutine(data.AlphaLoop(pointLifetime));
                });
            }
        }

        private void RemoveOldPoints()
        {
            float currentTime = Time.time;
            pointsData.ForEach(data =>
            {
                bool pointsRemoved = false;
                for (int i = data.positionsList.Count - 1; i >= 0; i--)
                {
                    if (currentTime - data.spawnTimestamp > pointLifetime)
                    {
                        data.positionsList.RemoveAt(i);
                        pointsRemoved = true;
                    }
                }

                if (pointsRemoved)
                {
                    ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
                }
            });
        }
    }
}
