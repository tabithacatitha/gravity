using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Scaler : MonoBehaviour
{
    [SerializeField] CanvasGroup scalarUI;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject line;
    private SelectableObject currentObject = null;
    private IScalable scalableObject = null;
    private float startingMouseDistance = 0f; // in world space
    private bool scaling = false;
    private new Camera camera;

    void Awake()
    {
        camera = Camera.main;
    }
    
    public bool isScaling()
    {
        return scaling;
    }

    public void BeginScaling(SelectableObject selectableObject)
    {
        currentObject = selectableObject;
        scalableObject = currentObject.GetComponent<IScalable>();
        scalableObject.SaveScale();

        // show scaling UI
        scalarUI.DOFade(1, 0.5f);
        scaling = true;

        // normalize(?) the scaling
        Vector2 startingMouseWorldPosition = camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        startingMouseDistance = (startingMouseWorldPosition - (Vector2) currentObject.transform.position).magnitude;

        UpdateScale();
    }

    public void CancelScaling()
    {
        if (scaling)
        {
            scalableObject.RestoreScale();
            EndScaling();
        }
    }

    public void EndScaling()
    {
        scaling = false;
        scalarUI.DOFade(0, 0.5f);

        currentObject = null;
        scalableObject = null;
    }
    
    void Update()
    {
        if (scaling)
        {
            UpdateScale();
        }
    }

    void UpdateScale()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 objectScreenPosition = camera.WorldToScreenPoint(currentObject.transform.position);
        Vector2 direction = Vector2.Normalize(mouseScreenPosition - objectScreenPosition);

        // arrow follows pointer in screen space
        arrow.transform.position = mouseScreenPosition;
        arrow.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 90);
        // arrow.transform.LookAt(objectScreenPosition, Vector3.back);

        // draw line between pointer and object center in screen space
        Vector2 size = new Vector2(5, Vector2.Distance(objectScreenPosition, mouseScreenPosition)); 
        Vector2 pos = (objectScreenPosition + mouseScreenPosition) / 2;
        line.GetComponent<RectTransform>().sizeDelta = size;
        line.transform.position = pos;
        line.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) - 90);


        // update object scale
        Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(mouseScreenPosition);
        Vector3 relativePosition = mouseWorldPosition - currentObject.transform.position;
        scalableObject.ScaleTo(relativePosition, startingMouseDistance);
    }
}
