using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MudarCena : MonoBehaviour
{

    public string destinationSceneName;
    public string arrivalSpawnName = "default";
    //public Transi��esdecena transicao;
    public void TrocarCena(string destinationSceneName)
   // private void Start()
    //{
    //    if (transicao == null)
     //   {
       //     transicao = FindObjectOfType<Transi��esdecena>();
    //}
   // }

    {
        Luzia.Instance.Luziavolta();
        Spawn.NextSpawnName = arrivalSpawnName;
        SceneManager.LoadScene(destinationSceneName);
        //transicao.nextSceneName = destinationSceneName;
        //StartCoroutine(transicao.FadeOutIn());

    }
}
