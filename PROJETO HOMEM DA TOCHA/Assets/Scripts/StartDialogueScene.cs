using DialogueEditor;
using UnityEngine;

public class StartDialogueScene : MonoBehaviour
{
    public NPCConversation myConversation;
    void Start()
    {
        ConversationManager.Instance.StartConversation(myConversation);
    }

}
