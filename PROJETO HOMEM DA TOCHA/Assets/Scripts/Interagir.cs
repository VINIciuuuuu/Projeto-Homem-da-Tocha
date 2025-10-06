using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Interagir : MonoBehaviour
{
    private bool podeinteragir;
    [Header("Acão do objeto")]
    [Tooltip("Olhe no código para ver cada ação")]
    public int action;
    /*
    Todas as a ações:

    0 - Destroi o objeto.
     o resto ainda vamos fazer

    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            podeinteragir = true;
        }
        else
        {
            podeinteragir = false;
        }
    }
    private void Update()
    {
        if (podeinteragir && Input.GetKeyDown(KeyCode.E))
        {
            if (action == 0)
            {
                Destroy(gameObject);
            }
            
            if (action == 3)
            {
                Destroy(gameObject); // apenas testei pra ver se funciona, e funcionou :)
            }
        }
    }
}
