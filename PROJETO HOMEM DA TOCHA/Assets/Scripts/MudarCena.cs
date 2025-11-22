using UnityEngine;
using UnityEngine.SceneManagement;
public class MudarCena : MonoBehaviour
{

    public string destinationSceneName;
    public string arrivalSpawnName = "default";
    public void TrocarCena(string destinationSceneName) 
    {
        Luzia.Instance.Luziavolta();
        Spawn.NextSpawnName = arrivalSpawnName;
        SceneManager.LoadScene(destinationSceneName);
    }
}
