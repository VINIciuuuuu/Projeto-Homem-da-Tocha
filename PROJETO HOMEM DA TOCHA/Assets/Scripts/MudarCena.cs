using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MudarCena : MonoBehaviour
{

    public string destinationSceneName;
    public string arrivalSpawnName = "default";
    public Transiçõesdecena transicao;

    private void Start()
    {
        if (transicao == null)
        {
            transicao = FindObjectOfType<Transiçõesdecena>();
        }
    }
    public void TrocarCena(string destinationSceneName)
    {
        Luzia.Instance.Luziavolta();
        Spawn.NextSpawnName = arrivalSpawnName;
        transicao.nextSceneName = destinationSceneName;
        StartCoroutine(transicao.FadeOutIn());

    }
}
