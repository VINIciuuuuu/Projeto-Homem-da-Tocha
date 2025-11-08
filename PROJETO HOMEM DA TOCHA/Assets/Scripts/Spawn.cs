using UnityEngine;
using UnityEngine.SceneManagement;

public class Spawn: MonoBehaviour
{
    private static Spawn Instance;

    public static string NextSpawnName;

    [Tooltip("Nome do spawn padrão desta cena")]
    public string defaultSpawnName = "default";

    [Tooltip("Se marcado, mantém o playter entre cenas")]
    public bool makePersistent = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (makePersistent)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        TryApplySpawn();
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance == this)
            TryApplySpawn();
    }

    void TryApplySpawn()
    {
        string nameToUse = !string.IsNullOrEmpty(NextSpawnName) ? NextSpawnName : defaultSpawnName;

        GameObject target = GameObject.Find(nameToUse);
        if (target != null)
        {
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
            NextSpawnName = null;
        }
        else
        {
            Debug.LogWarning(
                $"[PlayerSpawn2D] Spawn '{nameToUse}' não encontrado na cena '{SceneManager.GetActiveScene().name}'." + $"Crie um Empty com esse NOME ou ajuste 'defaultSpawnName'."
            );
        }
    }
}
