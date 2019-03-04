using Assets.Scripts.Utilities;
using UnityEngine;

public class MapCamera : MonoBehaviour
{

    public float stickMinZoom, stickMaxZoom;

    public float swivelMinZoom, swivelMaxZoom;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotationSpeed;
    private Transform swivel, stick;
    private float zoom = 1f;
    private float rotationAngle;
    private int tilesCountX;
    private int tilesCountY;

    private void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

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
