using DG.Tweening;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _optionsMenuPanel;

    private bool _optionsMenuActive;

    private void Awake()
    {
        CloseOptionsMenu();
    }

    public void OpenOptionsMenu(float delay = 0f)
    {
        if (_optionsMenuPanel == null) { return; }

        DOVirtual.DelayedCall(delay, () =>
        {
            _optionsMenuPanel.SetActive(true);
            _optionsMenuActive = true;
        });
    }

    public void CloseOptionsMenu(float delay = 0f)
    {
        if (_optionsMenuPanel == null) { return; }

        DOVirtual.DelayedCall(delay, () =>
        {
            _optionsMenuPanel.SetActive(false);
            _optionsMenuActive = false;
        });
    }
}
