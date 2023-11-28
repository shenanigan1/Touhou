using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject CreditMenu;
    public GameObject _MainMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        Time.timeScale = 0;
        PauseMenu.SetActive(true);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Play()
    {
        SceneManager.LoadScene(1);   
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Credit()
    {
        CreditMenu.SetActive(true);
        _MainMenu.SetActive(false);
    }

    public void CreditReturn()
    {
        CreditMenu.SetActive(false);
        _MainMenu.SetActive(true);
    }
}
