using UnityEngine;
using TMPro;
using DG.Tweening;

public class HideableMenu : MonoBehaviour
{
    [Header("Hiding Settings")]
    [SerializeField] Vector3 hideDistance;
    [SerializeField] float hideTime = 1f;

    [Header("Reference Links")]
    [SerializeField] TMP_Text buttonText;
    [SerializeField] string shownText = "<";
    [SerializeField] string hiddenText = ">";

    private bool hidden;

    Vector3 startingPosition;
    
    void Start()
    {
        startingPosition = this.transform.localPosition;
    }

    public void HidePressed()
    {
        DOTween.Kill(this, false);
        if (hidden)
        {
            this.transform.DOLocalMove(startingPosition, hideTime);
            if (buttonText != null) buttonText.text = shownText;
            hidden = false;
        } else
        {
            this.transform.DOLocalMove(startingPosition + hideDistance, hideTime);
            if (buttonText != null) buttonText.text = hiddenText;
            hidden = true;
        }
    }
}
