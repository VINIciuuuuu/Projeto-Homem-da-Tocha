using UnityEngine;

public class GameManagerscript : MonoBehaviour
{
    private GameObject Pausemenu;
    private bool IsPaused = false;
    [SerializeField] private GameObject menuprefab;

    private void Start()
    {
        if (Pausemenu != null)
        {
            Pausemenu.SetActive(false);
        }
    }

    public void Pausegame()
    {
        if (Pausemenu != null)
        {
            Pausemenu.SetActive(true);
            Time.timeScale = 0f;
            IsPaused = true;
        }
    }

    public void Resumegame()
    {
        if (Pausemenu != null)
        {
            Pausemenu.SetActive(false);
            Time.timeScale = 1f;
            IsPaused = false;
        }
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
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        GameObject[] Gamemanager = GameObject.FindGameObjectsWithTag("Gmanager");

        if (Gamemanager.Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            Pausemenu = Instantiate(menuprefab);
            DontDestroyOnLoad(Pausemenu);
        }
    }
}
