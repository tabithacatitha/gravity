using UnityEngine;
using UnityEngine.UI;

public class Fullscreen : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Sprite close;
    [SerializeField] Sprite open;

    void Awake()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    void Update()
    {
        if (Screen.fullScreen)
        {
            image.sprite = open;
        } else
        {
            image.sprite = close;
        }
    }
}
