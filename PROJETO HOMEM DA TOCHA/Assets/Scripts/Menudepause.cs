using UnityEngine;

public class Menudepause : MonoBehaviour
{
    public GameObject Pausemenu;
    private bool IsPaused = false;

    private void Start()
    {
        Pausemenu.SetActive(false);
    }

    public void Pausegame()
    {
        Pausemenu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resumegame()
    {
        Pausemenu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Resumegame();
            }
            else
            {
                Pausegame();
            }
        }
        DontDestroyOnLoad(Pausemenu);
    }
}
