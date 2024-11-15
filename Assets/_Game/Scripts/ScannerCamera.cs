using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace LRS
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScannerCamera : MonoBehaviour
    {
        /*
        private InputAction _fire;
        private LineRenderer _lineRenderer;

        [SerializeField] private List<PointsData> pointsData = new();

        private const string REJECT_LAYER_NAME = "PointReject";
        private const string TEXTURE_NAME = "PositionsTexture";
        private const string RESOLUTION_PARAMETER_NAME = "Resolution";

        [SerializeField] private bool reuseOldParticles = false;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject vfxContainer;
        [SerializeField] private Transform castPoint;
        [SerializeField] private float radius = 10f;
        [SerializeField] private int pointsPerScan = 1000; // Increased number of points for a "flash" effect
        [SerializeField] private float range = 10f;
        [SerializeField] private int resolution = 100;
        [SerializeField] private float pointLifetime = 5f; // Duration in seconds
        [SerializeField] private float cooldownTime = 2f; // Cooldown duration in seconds


        private bool _createNewVFX;
        private bool _isCooldown;
        private float _lastFireTime;

        private void Start()
        {
            _fire = playerInput.actions["Fire"];

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
            for (int i = 0; i < pointsData.Count; i++)
            {
                var data = pointsData[i];
                ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
            }
        }

        private void ApplyPositions(List<Vector3> positionsList, VisualEffect currentVFX, Texture2D texture, Color[] positions)
        {
            Vector3[] pos = positionsList.ToArray();
            Vector3 vfxPos = currentVFX.transform.position;
            int loopLength = texture.width * texture.height;
            int posListLen = pos.Length;
            float currentTime = Time.time;

            for (int i = 0; i < loopLength; i++)
            {
                Color data;

                if (i < posListLen)
                {
                    float alpha = pointsData[0].GetAlpha(currentTime, pointLifetime, i); // Assuming all pointsData have the same lifetime
                    data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, alpha);
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

        private void Scan()
        {
            if (_fire.IsPressed() && !_isCooldown)
            {
                _isCooldown = true;
                _lastFireTime = Time.time;

                for (int i = 0; i < pointsPerScan; i++)
                {
                    Vector3 randomPoint = Random.insideUnitSphere * radius;
                    randomPoint += castPoint.position;
                    Vector3 dir = (randomPoint - transform.position).normalized;

                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range, layerMask))
                    {
                        if (hit.collider.CompareTag(REJECT_LAYER_NAME)) continue;

                        int resolution2 = resolution * resolution;
                        pointsData.ForEach(data =>
                        {
                            data.includedTags.ForEach(tag =>
                            {
                                if (hit.collider.CompareTag(tag))
                                {
                                    if (data.positionsList.Count < resolution2)
                                    {
                                        data.positionsList.Add(hit.point);
                                        data.timestamps.Add(Time.time);
                                    }
                                    else if (reuseOldParticles)
                                    {
                                        data.positionsList.RemoveAt(0);
                                        data.timestamps.RemoveAt(0);
                                        data.positionsList.Add(hit.point);
                                        data.timestamps.Add(Time.time);
                                    }
                                    else
                                    {
                                        _createNewVFX = true;
                                        data.currentVisualEffect = NewVisualEffect(data.prefab, out data.texture, out data.positionsAsColors);
                                        data.positionsList.Clear();
                                        data.timestamps.Clear();
                                    }
                                }
                            });
                        });
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, dir * range, Color.red);
                    }
                }

                pointsData.ForEach(data =>
                {
                    ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
                });
            }

            if (_isCooldown && Time.time - _lastFireTime >= cooldownTime)
            {
                _isCooldown = false;
            }
        }

        private void RemoveOldPoints()
        {
            float currentTime = Time.time;
            for (int i = 0; i < pointsData.Count; i++)
            {
                var data = pointsData[i];
                bool pointsRemoved = false;
                for (int j = data.timestamps.Count - 1; j >= 0; j--)
                {
                    if (currentTime - data.timestamps[j] > pointLifetime)
                    {
                        if (j < data.positionsList.Count)
                        {
                            data.positionsList.RemoveAt(j);
                        }
                        data.timestamps.RemoveAt(j);
                        pointsRemoved = true;
                    }
                }
                if (pointsRemoved)
                {
                    ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
                }
            }
        }
        */
    }
}
