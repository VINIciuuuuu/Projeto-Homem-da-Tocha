using UnityEngine;
using UnityEngine.SceneManagement;

public class InternalIventory : MonoBehaviour
{
    public bool vara = false;
    public bool pano = false;

    public void IntVarapegou()
    {
        vara = true;
    }

    public void IntPanopegou()
    {
        pano = true;
    }

    public void Tpjanela()
    {
        if (pano)
        {
            SceneManager.LoadSceneAsync(12);
        }
    }
}