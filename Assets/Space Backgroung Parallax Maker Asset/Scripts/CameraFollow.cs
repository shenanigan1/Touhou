using UnityEngine;

namespace Mkey
{
    /// <summary>
    /// Enum describing different camera tracking modes.
    /// </summary>
    public enum TrackMode { Player, Mouse, Gyroscope, Keyboard, Touch }

    /// <summary>
    /// CameraFollow handles smooth camera movement tracking the player, mouse, gyroscope, keyboard or touch input.
    /// Supports clamping camera position within a defined 2D box.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        // Margins to define how far the player can move before the camera starts following
        private Vector2 margin;
        // Smoothness factor for camera movement
        private Vector2 smooth;

        // Current tracking mode for the camera
        public TrackMode track = TrackMode.Touch;

        // Enables or disables clamping of the camera position within the ClampField bounds
        public bool ClampPosition;

        // BoxCollider2D defining camera movement boundaries if ClampPosition is enabled
        public BoxCollider2D ClampField;

        [SerializeField]
        private GameObject player;

        private Camera m_camera;

        // Camera orthographic sizes for clamping calculations
        private float camVertSize;
        private float camHorSize;

        // Smoothed acceleration vector for gyroscope or mouse tracking
        private Vector3 acceleration;

        [SerializeField]
        private float speedMultiplicator = 1f;

        [SerializeField]
        private bool InGame = true;

        /// <summary>
        /// Returns the current screen width/height ratio.
        /// </summary>
        public float ScreenRatio => (float)Screen.width / Screen.height;

        /// <summary>
        /// Singleton instance for easy global access.
        /// </summary>
        public static CameraFollow Instance;

        #region Unity Callbacks

        private void Awake()
        {
            // Auto-assign player if not set via inspector
            if (!player)
                player = GameObject.FindGameObjectWithTag("Player");

            margin = new Vector2(3f, 3f);
            smooth = new Vector2(1f, 1f);

            m_camera = GetComponent<Camera>();
            Instance = this;
        }

        private void Start()
        {
            // Subscribe to touch drag event if implemented elsewhere
            // TouchPad.Instance.ScreenDragEvent += TrackTouchDrag;
        }

        private void LateUpdate()
        {
            switch (track)
            {
                case TrackMode.Player:
                    TrackPlayer();
                    break;
                case TrackMode.Mouse:
                    TrackMouseMotion();
                    break;
                case TrackMode.Gyroscope:
                    TrackGyroscope();
                    break;
                case TrackMode.Keyboard:
                    TrackKeyboard();
                    break;
                    // Touch handled by event
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from touch drag event to avoid memory leaks
            // TouchPad.Instance.ScreenDragEvent -= TrackTouchDrag;
        }

        #endregion

        #region Tracking Implementations

        /// <summary>
        /// Checks if player is out of horizontal margin.
        /// </summary>
        private bool OutOfXMargin => Mathf.Abs(transform.position.x - player.transform.position.x) > margin.x;

        /// <summary>
        /// Checks if player is out of vertical margin.
        /// </summary>
        private bool OutOfYMargin => Mathf.Abs(transform.position.y - player.transform.position.y) > margin.y;

        /// <summary>
        /// Tracks the player with smooth lerp if they move out of margin.
        /// </summary>
        private void TrackPlayer()
        {
            if (!player)
                return;

            float targetX = transform.position.x;
            float targetY = transform.position.y;

            if (OutOfXMargin)
                targetX = Mathf.Lerp(player.transform.position.x, transform.position.x, smooth.x * Time.deltaTime);

            if (OutOfYMargin)
                targetY = Mathf.Lerp(player.transform.position.y, transform.position.y, smooth.y * Time.deltaTime);

            transform.position = new Vector3(targetX, targetY, transform.position.z);

            ClampCameraPosInField();
        }

        /// <summary>
        /// Tracks the mouse cursor movement by smoothing camera position accordingly.
        /// </summary>
        private void TrackMouseMotion()
        {
            acceleration = Vector3.Lerp(acceleration, Camera.main.ScreenToViewportPoint(Input.mousePosition), Time.deltaTime);

            Vector3 target = transform.position + new Vector3(acceleration.x - 0.5f, acceleration.y - 0.5f, 0f);

            transform.position = Vector3.Lerp(transform.position, target, 5f * Time.deltaTime);

            ClampCameraPosInField();
        }

        /// <summary>
        /// Tracks the gyroscope acceleration input, smoothing camera movement.
        /// </summary>
        private void TrackGyroscope()
        {
            Vector3 dir = new Vector3(Input.acceleration.x, Input.acceleration.y, 0f);

            if (dir.sqrMagnitude > 1f)
                dir.Normalize();

            acceleration = dir;

            Vector3 target = transform.position + new Vector3(acceleration.x, acceleration.y, 0f);

            transform.position = Vector3.Lerp(transform.position, target, 1f * Time.deltaTime);

            ClampCameraPosInField();
        }

        /// <summary>
        /// Tracks keyboard input movement and moves camera accordingly.
        /// Assumes player has a PlayerController component with GetMovement method.
        /// </summary>
        private void TrackKeyboard()
        {
            if (!InGame || player == null)
                return;

            Vector3 dir = player.GetComponent<PlayerController>().GetMovement();

            // Only vertical movement affects camera here, adjust if needed
            Vector3 target = transform.position + new Vector3(0f, -dir.y * speedMultiplicator, 0f);

            transform.position = Vector3.Lerp(transform.position, target, 1f * Time.deltaTime);

            ClampCameraPosInField();
        }

        /// <summary>
        /// Tracks touch drag direction. Intended to be called via event.
        /// </summary>
        /// <param name="tpea">TouchPad event args providing drag direction.</param>
        private void TrackTouchDrag(TouchPadEventArgs tpea)
        {
            if (track != TrackMode.Touch)
                return;

            Vector3 dir = tpea.DragDirection;

            Vector3 target = transform.position + new Vector3(dir.x, dir.y, 0f);

            transform.position = Vector3.Lerp(transform.position, target, 0.02f * Time.fixedDeltaTime);

            ClampCameraPosInField();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Clamps the camera position inside the ClampField BoxCollider2D bounds,
        /// accounting for camera orthographic size and screen ratio.
        /// </summary>
        private void ClampCameraPosInField()
        {
            if (!ClampPosition || ClampField == null || m_camera == null)
                return;

            camVertSize = m_camera.orthographicSize;
            camHorSize = camVertSize * ScreenRatio;

            Vector2 halfBoundsSize = ClampField.bounds.size / 2f;
            halfBoundsSize -= new Vector2(camHorSize, camVertSize);

            Vector3 clampCenter = ClampField.transform.position;

            float maxX = clampCenter.x + halfBoundsSize.x;
            float minX = clampCenter.x - halfBoundsSize.x;
            float maxY = clampCenter.y + halfBoundsSize.y;
            float minY = clampCenter.y - halfBoundsSize.y;

            float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
            float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }

        #endregion
    }
}
