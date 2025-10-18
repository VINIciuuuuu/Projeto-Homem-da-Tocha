using UnityEngine;
using UnityEngine.SceneManagement;

public class InternalIventory : MonoBehaviour
{
    public bool vara = false;
    public bool pano = false;
    public bool chavedaporta = false;
    public Dialogdoor Scriptportapais;
    public Dialogdooraberta Scriptportapaisaberta;
    public tpportafora Scripttpportafora;

    private void Start()
    {
        Scriptportapais = FindAnyObjectByType<Dialogdoor>();
        Scriptportapaisaberta = FindAnyObjectByType<Dialogdooraberta>();
        Scripttpportafora = FindAnyObjectByType<tpportafora>();
    }

    public void IntVarapegou()
    {
        vara = true;
    }

    public void IntPanopegou()
    {
        pano = true;
    }

    public void IntChaveportapaispegou()
    {
        chavedaporta=true;
    }

    public void Tpjanela()
    {
        if (pano)
        {
            SceneManager.LoadSceneAsync(3);
        }
    }

    public void Update()
    {
        if (vara)
        {
            Scriptportapais.Pegouvara();
            Scriptportapaisaberta.Pegouvara();
        }

        if (chavedaporta)
        {
            Scripttpportafora.pegouchavedoor();
        }
    }
}