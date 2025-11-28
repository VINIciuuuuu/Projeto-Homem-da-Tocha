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
    [Tooltip("Tecla para interagir com a porta")]
    public KeyCode InteractionKey = KeyCode.E;

    [Header("Feedback Visual (Opcional)")]
    [Tooltip("GameObject do botão E visual. Deixe vazio se não quiser usar.")]
    public GameObject BotaoE;
    private SpriteRenderer botaoSR;

    private bool Podeinteragir = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        // Busca o botão E visual se não foi atribuído
        if (BotaoE == null)
        {
            BotaoE = GameObject.FindGameObjectWithTag("BotaoEicon");
        }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Verifica se esta porta tem um LockedDoor (requer urtigas)
        LockedDoor lockedDoor = GetComponent<LockedDoor>();
        
        if (lockedDoor != null)
        {
            // Esta porta requer urtigas - verifica se pode passar
            if (!lockedDoor.PodePassar())
            {
                // Player não tem urtigas suficientes - mostra diálogo e não permite interação
                lockedDoor.ExibirDialogoBloqueio();
                return; // Não marca como pode interagir
            }
        }

        // Se chegou aqui, pode interagir - marca como disponível
        Podeinteragir = true;

        // Mostra o botão E visual
        if (botaoSR != null)
        {
            Color cor = botaoSR.color;
            cor.a = 1f;
            botaoSR.color = cor;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Podeinteragir = false;

        // Esconde o botão E visual
        if (botaoSR != null)
        {
            Color cor = botaoSR.color;
            cor.a = 0f;
            botaoSR.color = cor;
        }
    }

    void Update()
    {
        // Verifica se o player pressionou a tecla de interação
        if (Input.GetKeyDown(InteractionKey) && Podeinteragir)
        {
            TentarTeleportar();
        }
    }

    /// <summary>
    /// Tenta teleportar o player para a cena de destino
    /// </summary>
    private void TentarTeleportar()
    {
        // Verifica novamente se tem LockedDoor (caso tenha mudado durante a interação)
        LockedDoor lockedDoor = GetComponent<LockedDoor>();
        
        if (lockedDoor != null)
        {
            // Verifica se ainda pode passar (pode ter mudado desde que entrou no trigger)
            if (!lockedDoor.PodePassar())
            {
                // Player não tem urtigas suficientes - bloqueia a passagem
                lockedDoor.ExibirDialogoBloqueio();
                return; // Impede a mudança de cena
            }
        }

        // Se chegou aqui, pode teleportar
        Spawn.NextSpawnName = arrivalSpawnName;
        SceneManager.LoadScene(destinationSceneName);
    }
}


