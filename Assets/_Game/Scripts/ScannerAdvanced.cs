/*
 * Author: Leonhard Robin Schnaitl
 * GitHub: https://github.com/leonhardrobin
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace LRS
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScannerAdvanced : MonoBehaviour
    {
        private InputAction _fire;
        private InputAction _changeRadius;
        private LineRenderer _lineRenderer;

        [SerializeField] private List<PointsData> pointsData = new();

        private const string REJECT_LAYER_NAME = "PointReject";
        private const string TEXTURE_NAME = "PositionsTexture";
        private const string RESOLUTION_PARAMETER_NAME = "Resolution";

        [SerializeField] private LayerMask layerMask;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject vfxContainer;
        [SerializeField] private Transform castPoint;
        [SerializeField] private float radius = 10f;
        [SerializeField] private float maxRadius = 10f;
        [SerializeField] private float minRadius = 1f;
        [SerializeField] private int pointsPerScan = 100;
        [SerializeField] private float range = 10f;
        [SerializeField] private int resolution = 100;
        [SerializeField] private float pointLifetime = 5f; // Duration in seconds

        [SerializeField] private float cooldownTime = 6f; // Cooldown duration in seconds
        private bool _isCooldown;
        private float _lastFireTime;

        private bool _createNewVFX;

        private void Start()
        {
            _fire = playerInput.actions["Fire"];
            _changeRadius = playerInput.actions["Scroll"];
 
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
            ChangeRadius();
            RemoveOldPoints();
        }

        private void ChangeRadius()
        {
            if (_changeRadius.triggered)
            {
                radius = Mathf.Clamp(radius + _changeRadius.ReadValue<float>() * Time.deltaTime, minRadius, maxRadius);
            }
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
                    Vector3 randomPoint = Random.insideUnitSphere * radius;
                    randomPoint += castPoint.position;
                    Vector3 dir = (randomPoint - transform.position).normalized;

                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range, layerMask))
                    {
                        if (hit.collider.CompareTag(REJECT_LAYER_NAME)) continue;

                        int resolution2 = resolution * resolution;
                        pointsData.ForEach(data =>
                        {
                            data.spawnTimestamp = Time.time; // Set spawntime
                            data.includedTags.ForEach(tag => // Check om farven matcher target
                            {
                                if (hit.collider.CompareTag(tag)) // Check om farven matcher det man rammer
                                {
                                    data.positionsList.Add(hit.point);
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

