using UnityEngine;
using System.Collections.Generic;

namespace Mkey
{
    /// <summary>
    /// Creates a parallax scrolling effect along the X-axis.
    /// Supports infinite scrolling planes.
    /// </summary>
    public class SpriteParallax : MonoBehaviour
    {
        [Header("Parallax Settings")]
        [Tooltip("Base speed of parallax scrolling.")]
        [SerializeField] private float speedParralax = 0f;

        [Tooltip("Array of parallax planes to move.")]
        [SerializeField] private ParallaxPlane[] planes;

        [Tooltip("Enable infinite scrolling for planes.")]
        [SerializeField] private bool infiniteMap = true;

        [Tooltip("Width of the infinite map (used if infiniteMap is true).")]
        [SerializeField] private float mapSizeX = 20.48f;

        [Tooltip("Height of the infinite map (used if infiniteMap is true).")]
        [SerializeField] private float mapSizeY = 20.48f;

        [Tooltip("Relative offset for the first plane (0 to 1).")]
        [SerializeField] private float firstPlaneRelativeOffset = 0f;

        [Tooltip("Relative offset for the last plane (0 to 1).")]
        [SerializeField] private float lastPlaneRelativeOffset = 0.9f;

        private Transform m_Camera;
        private Vector3 camPos;      // Current camera position
        private Vector3 oldCamPos;   // Previous frame camera position
        private int length = 0;      // Number of planes

        private float[] planeOffset;

        // These lists are declared but never used - consider removing or implementing later
        private List<ParallaxPlane> InfiniteGroup;
        private List<ParallaxPlane>[] InfiniteMap;

        /// <summary>
        /// Validate serialized fields in editor to ensure they stay within valid ranges.
        /// </summary>
        private void OnValidate()
        {
            firstPlaneRelativeOffset = Mathf.Clamp01(firstPlaneRelativeOffset);
            lastPlaneRelativeOffset = Mathf.Clamp01(lastPlaneRelativeOffset);
        }

        /// <summary>
        /// Initialize camera reference, plane offsets, and infinite map setup.
        /// </summary>
        private void Start()
        {
            m_Camera = Camera.main.transform;
            camPos = m_Camera.position;
            oldCamPos = camPos;

            length = planes.Length;

            // Clamp offsets to [0,1]
            firstPlaneRelativeOffset = Mathf.Clamp01(firstPlaneRelativeOffset);
            lastPlaneRelativeOffset = Mathf.Clamp01(lastPlaneRelativeOffset);

            // Calculate offset step between planes
            float offsetStep = Mathf.Abs(lastPlaneRelativeOffset - firstPlaneRelativeOffset) / (length - 1);
            planeOffset = new float[length];

            // Cache relative offsets for each plane
            for (int i = 0; i < length; i++)
            {
                var plane = planes[i];
                if (plane == null) continue;

                planeOffset[i] = firstPlaneRelativeOffset + i * offsetStep;
            }

            // If infinite scrolling is enabled, initialize planes accordingly
            if (infiniteMap)
            {
                for (int i = 0; i < length; i++)
                {
                    if (planes[i] != null)
                        planes[i].CreateInfinitePlane(new Vector2(mapSizeX, mapSizeY), camPos);
                }
            }
        }

        /// <summary>
        /// Update parallax effect each frame based on camera movement.
        /// </summary>
        private void Update()
        {
            camPos = m_Camera.position;
            Vector3 camOffset = camPos - oldCamPos;

            for (int i = 0; i < length; i++)
            {
                var plane = planes[i];
                if (plane == null) continue;

                // Calculate movement vector for this plane
                Vector3 movement = new Vector3(camOffset.x * planeOffset[i], camOffset.y * planeOffset[i] + speedParralax, 0);

                // Smoothly move plane towards target position based on camera offset
                plane.transform.position = Vector3.Lerp(plane.transform.position, plane.transform.position + movement, Time.deltaTime);

                // Update infinite plane if enabled
                if (infiniteMap)
                    plane.UpdateInfinitePlane(camPos);
            }

            oldCamPos = camPos;
        }
    }
}
