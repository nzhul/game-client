using Assets.Scripts.Utilities;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    #region Singleton
    private static MapCamera _instance;

    public static MapCamera Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }
    #endregion

    public float stickMinZoom, stickMaxZoom;

    public float swivelMinZoom, swivelMaxZoom;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotationSpeed;
    private Transform swivel, stick;
    private float zoom = 1f;
    private float rotationAngle;
    private int tilesCountX;
    private int tilesCountY;

    // Following
    private Vector3 followTarget;
    private bool isFollowingTarget;
    private float smoothSpeed = 0.3f;
    private Vector3 velocity = Vector3.zero;

    //private void Awake()
    //{
    //    swivel = transform.GetChild(0);
    //    stick = swivel.GetChild(0);
    //}

    private void Start()
    {
        MapManager.Instance.OnInitComplete += Graph_OnGraphInitialized;
    }

    private void Graph_OnGraphInitialized()
    {
        Graph graph = MapManager.Instance.graph;

        if (graph != null)
        {
            tilesCountX = graph.graphSizeX;
            tilesCountY = graph.graphSizeY;
        }
    }

    private void Update()
    {
        if (!Common.IsMouseOverUI())
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f)
            {
                AdjustZoom(zoomDelta);
            }

            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f)
            {
                AdjustRotation(rotationDelta);
            }

            float xDelta = Input.GetAxis("Horizontal");
            float zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f)
            {
                AdjustPosition(xDelta, zDelta);
            }
        }

        if (this.isFollowingTarget && followTarget != Vector3.zero)
        {
            FollowTarget();
        }
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = followTarget;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        this.transform.position = smoothedPosition;

        Vector3 epsilon = desiredPosition - this.transform.position;

        if (Mathf.Abs(epsilon.x) < 0.5 && Mathf.Abs(epsilon.z) < 0.5)
        {
            this.isFollowingTarget = false;
            this.followTarget = Vector3.zero;
        }
    }

    public void StartFollowTarget(Vector3 target)
    {
        this.isFollowingTarget = true;
        this.followTarget = new Vector3(target.x, this.transform.position.y, target.z);
    }

    private void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    private void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction =
            transform.localRotation *
            new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance =
            Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
            damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        // TODO: use the values of the actual map when we have real tiles
        // in order to limit the region where the camera can go.
        float tilesCountX = this.tilesCountX;
        float tilesCountZ = this.tilesCountY;
        float tileSizeX = 1f;
        float tileSizeZ = 1f;

        float xMax = (tilesCountX * tileSizeX);
        position.x = Mathf.Clamp(position.x, -xMax, xMax);

        float zMax = (tilesCountZ * tileSizeZ);
        position.z = Mathf.Clamp(position.z, -zMax, zMax);

        return position;
    }
}
