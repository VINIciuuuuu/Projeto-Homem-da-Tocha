using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Destino")]
    public string destinationSceneName;
    public string arrivalSpawnName = "default";
    
    [Header("Interação")]
    public KeyCode InteractionKey = KeyCode.E;
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;
    private bool Podeinteragir;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
    
    void Start()
    {
        BotaoE = GameObject.FindGameObjectWithTag("BotaoEicon");

        if (BotaoE != null)
        {
            botaoSR = BotaoE.GetComponent<SpriteRenderer>();

            if (botaoSR != null)
            {
                Color cor = botaoSR.color;
                cor.a = 0f;
                botaoSR.color = cor;
            }
        }
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        Podeinteragir = true;
        
        if (botaoSR != null)
        {
            Color cor = botaoSR.color;
            cor.a = 1f;
            botaoSR.color = cor;
        }
    }
    
    public void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        Podeinteragir = false;
        
        if (botaoSR != null)
        {
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(InteractionKey) && Podeinteragir)
        {
            Spawn.NextSpawnName = arrivalSpawnName;
            SceneManager.LoadScene(destinationSceneName);
        }
    }
}


