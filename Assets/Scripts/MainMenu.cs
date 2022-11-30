using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject SettingsPanel;
    public GameObject LoadingPanel;
    public void StartGame()
    {
        MenuPanel.SetActive(false);
        LoadingPanel.SetActive(true);
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void SwitchPanel() 
    {
        if (MenuPanel.activeInHierarchy)
        {
            MenuPanel.SetActive(false);
            SettingsPanel.SetActive(true);
        }
        else
        {
            SettingsPanel.SetActive(false);
            MenuPanel.SetActive(true);
        }
    }
    public void QuitApplication()
    {
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
