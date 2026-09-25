using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class ClickManager : MonoBehaviour
{
    [SerializeField] Scaler scaler;
    [SerializeField] CanvasGroup selectedMenu;
    [SerializeField] float selectHideTime = 1f;
    SelectableObject selectedObject = null;
    SelectableObject mouseDownObject = null;
    public event Action<SelectableObject> selectedCallback;
    private static ClickManager instance;
    public static ClickManager Instance
    {
        get { return instance; }
        set {}
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Debug.LogError("more than one ClickManager");
        }

        HideButtonBar(selectedObject == null);
    }

    public void PointerDown(SelectableObject source)
    {
        if (scaler.isScaling())
        {
            scaler.EndScaling();
        } else
        {
            mouseDownObject = source;
        }
    }

    public void PointerUp(SelectableObject source)
    {
        if (mouseDownObject == source)
        {
            UpdateSelectableObjects(source);
        } else
        {
            UpdateSelectableObjects(null);
        }
        mouseDownObject = null;
    }

    void UpdateSelectableObjects(SelectableObject selectedObject)
    {
        HideButtonBar(selectedObject == null);
        selectedCallback(selectedObject);
        this.selectedObject = selectedObject;
    }

    // User Inputs

    public void SpawnPrefab(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab);
        SelectableObject selectableObject = instance.GetComponent<SelectableObject>();
        selectableObject.RandomizePosition();
        selectableObject.UpdateSelected(selectableObject);
        UpdateSelectableObjects(selectableObject);
    }

    void OnDelete()
    {
        if (scaler.isScaling()) scaler.EndScaling();
        if (selectedObject == null) return;
        Destroy(selectedObject.gameObject);
        UpdateSelectableObjects(null);
    }

    void OnScale()
    {
        if (selectedObject == null) return;
        if (scaler.isScaling()) {
            scaler.EndScaling();
        } else {
            if (!selectedObject.Enabled())
            {
                selectedObject.Enable(true);
            }
            scaler.BeginScaling(selectedObject);
        }
    }

    void OnCancel()
    {
        if (scaler.isScaling())
        {
            scaler.CancelScaling();
        }
    }

    void OnEnable()
    {
        if (selectedObject == null) return;
        if (scaler.isScaling()) scaler.CancelScaling();
        selectedObject.ToggleEnable();
    }

    void HideButtonBar(bool hide)
    {
        DOTween.Kill(selectedMenu, false);
        if (hide)
        {
            // DOTween.To(()=> selectedMenu.alpha, x=> selectedMenu.alpha = x, 0, selectHideTime);
            selectedMenu.DOFade(0, selectHideTime);
        } else
        {
            // DOTween.To(()=> selectedMenu.alpha, x=> selectedMenu.alpha = x, 1, selectHideTime);
            selectedMenu.DOFade(1, selectHideTime);
        }
    }
}
