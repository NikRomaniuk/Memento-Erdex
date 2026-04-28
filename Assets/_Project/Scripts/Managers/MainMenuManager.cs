using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _optionsMenuPanel;

    private bool _optionsMenuActive;

    private void Awake()
    {
        CloseOptionsMenu();
    }

    public void OpenOptionsMenu()
    {
        if (_optionsMenuPanel == null) { return; }

        _optionsMenuPanel.SetActive(true);
        _optionsMenuActive = true;
    }

    public void CloseOptionsMenu()
    {
        if (_optionsMenuPanel == null) { return; }

        _optionsMenuPanel.SetActive(false);
        _optionsMenuActive = false;
    }
}
