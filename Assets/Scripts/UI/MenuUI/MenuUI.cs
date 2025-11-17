using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [Header("Local UI References")]
    [SerializeField] Button StartButton;
    [SerializeField] Button OptionsButton;
    [SerializeField] Button QuitButton;
    [SerializeField] GameObject Visual;
    [SerializeField] OptionsMenuUI OptionsMenuUIRef;

    private bool IsActive{ get { return (Visual.activeSelf); } }

    private SimGameManager simManager;

    private void Start()
    {
        simManager = SimGameManager.Instance;

        InputHandler.Instance.OnPauseAction += Instance_OnPauseAction;

        StartButton.onClick.AddListener(() => { UnPause(); });
        OptionsButton.onClick.AddListener(() => { Hide();OptionsMenuUIRef.Show(); });
        QuitButton.onClick.AddListener(() => { Application.Quit(); });

        Hide();
        Pause();
    }

    private void Instance_OnPauseAction(object sender, System.EventArgs e)
    {
        if(IsActive)
        {
             // UnPause();
        }
        else
        {
            Pause();
        }
    }

    private void Pause()
    {
        Show();
        Time.timeScale = 0;
        simManager.PauseGame();
        simManager.SetMenuState(true);
    }

    private void UnPause()
    {
        Time.timeScale = 1;
        simManager.StartGame();
        simManager.SetMenuState(false);
        Hide();
    }

    public void Show()
    {
        Visual.SetActive(true);
    }
    public void Hide()
    {
        Visual.SetActive(false);
    }
}
