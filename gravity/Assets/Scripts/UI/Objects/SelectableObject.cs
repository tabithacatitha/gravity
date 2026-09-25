using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(IScalable))]
public class SelectableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Selection Settings")]
    [SerializeField] bool showSelector = true;
    [SerializeField] GameObject disableChild;

    [Header("Object Settings")]
    [SerializeField] public float radius = 0.5f;


    private new static Camera camera = null;
    private static ClickManager clickManager = null;
    private static Vector2 min;
    private static Vector2 max;
    private Renderer rend;
    private Color baseColor;
    private Color centerColor;
    private new bool enabled = true;
    
    void Awake()
    {
        if (camera == null) {
            camera = Camera.main;
            min = camera.ScreenToWorldPoint(Vector2.zero);
            max = camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, camera.pixelHeight));
        }
        if (showSelector)
        {
            rend = GetComponent<Renderer>();
            baseColor = rend.material.GetColor("_Color");
            centerColor = rend.material.GetColor("_CenterColor");
        }
    }

    void Start()
    {
        clickManager = ClickManager.Instance;
        clickManager.selectedCallback += UpdateSelected;
    }

    private bool selected = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        clickManager.PointerDown(this);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.hovered.Contains(this.gameObject)) {
            if (showSelector)
            {
                clickManager.PointerUp(this);
            } else
            {
                clickManager.PointerUp(null);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (selected && eventData.pointerCurrentRaycast.worldPosition != Vector3.zero) {
            // Debug.Log(eventData.pointerCurrentRaycast.worldPosition);
            this.transform.position = SnapWithinBounds(eventData.pointerCurrentRaycast.worldPosition, this.radius, min, max);
        }
    }

    public void ToggleEnable()
    {
        enabled = !enabled;
        Enable(enabled);
    }

    public void Enable(bool enabled)
    {
        if (showSelector)
        {
            if (enabled)
            {
                GetComponent<IScalable>().Enable();
                rend.material.SetColor("_Color", baseColor);
            } else
            {
                GetComponent<IScalable>().Disable();
                Color newColor = Color.gray;
                newColor.a = baseColor.a;
                rend.material.SetColor("_Color", newColor);
            }
        }
    }

    public bool Enabled()
    {
        return enabled;
    }

    private static Vector2 SnapWithinBounds(Vector2 value, float radius, Vector2 min, Vector2 max)
    {
        value = Vector2.Max(value, min + new Vector2(radius, radius));
        value = Vector2.Min(value, max - new Vector2(radius, radius));
        return value;
    }

    public void UpdateSelected(SelectableObject selectableObject) {
        this.selected = selectableObject == this;
        if (showSelector)
        {
            if (this.selected)
            {
                Color newCenterColor = Color.white;
                newCenterColor.a = 0.5f;
                rend.material.SetColor("_CenterColor", newCenterColor);
            } else
            {
                rend.material.SetColor("_CenterColor", centerColor);
            }
        }
    }

    public void RandomizePosition()
    {
        Vector2 minPos = min + new Vector2(radius, radius);
        Vector2 maxPos = max - new Vector2(radius, radius);
        this.transform.position = new Vector2(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y));
    }

    void OnDestroy()
    {
        clickManager.selectedCallback -= UpdateSelected;
    }
}