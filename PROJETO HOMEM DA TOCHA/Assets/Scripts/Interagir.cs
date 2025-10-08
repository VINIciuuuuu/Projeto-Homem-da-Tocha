using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Interagir : MonoBehaviour
{
    [SerializeField] private int sceneDialogo = 15;
    private bool dialogoAberto = false;
    private bool podeinteragir;
    [Header("Acão do objeto")]
    [Tooltip("Olhe no código para ver cada ação")]
    public int action;
    public int cena;
    public Luzia scriptLuzia;

    /*
    Todas as a ações:

    0 - Destroi o objeto.
    1 - Interagir resulta em trocar de cena
    2 - interação
     o resto ainda vamos fazer

    */

    private void Start()
    {
        scriptLuzia = FindAnyObjectByType<Luzia>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            podeinteragir = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            podeinteragir = false;
            dialogoAberto = false;
        }
    }

    private void Update()
    {
 
        if (podeinteragir && Input.GetKeyDown(KeyCode.E )|| podeinteragir && Input.GetKeyDown(KeyCode.Return))
        {
            if (action == 0)
            {
                Destroy(gameObject);
            }
            
            if (action == 1)
            {
                SceneManager.LoadSceneAsync(cena);
            }

            if (action == 2)
            {
                if (!dialogoAberto)
                {
                    SceneManager.LoadScene(sceneDialogo, LoadSceneMode.Additive);
                    dialogoAberto = true;
                }
            }
        }
    }

    public void DialogoFechado()
    {
        dialogoAberto = false;
        SceneManager.UnloadSceneAsync(15);
        //Está função é para resetar a bool dialogoAberto para que outro possa ser aberto
        //MoveSpeed volta ao normal quando o dialogo é fechado
        //UnloadSceneAsyn fecha a cena de dialog q foi aberta
        //Está função será aberta no DialogManager da cena de dialogo.
    }
}
