using UnityEngine;
using DialogueEditor;

public class NPCKey : MonoBehaviour
{
    public NPCConversation myConversation;
    public KeyCode InteractionKey = KeyCode.E;
    private bool Podeinteragir;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Podeinteragir = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Podeinteragir = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(InteractionKey) && Podeinteragir)
        {
            ConversationManager.Instance.StartConversation(myConversation);
        }
    }
}
