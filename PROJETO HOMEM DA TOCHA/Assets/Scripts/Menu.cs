using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Transiçõesdecena : MonoBehaviour
{
    public Image fadeImage; // Coloque a Image preta aqui
    public float fadeSpeed = 1f; // Velocidade do fade
    public string nextSceneName; // Nome da cena para mudar
    public GameObject FadeinCanva;
    public GameObject buttonmenu;

    bool isFading = false;

    void Awake()
    {
        FadeinCanva.SetActive(false);

        if (FadeinCanva == null)
        {
            GameObject.FindGameObjectWithTag("FadeIn");
        }

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(FadeinCanva);
    }

    // Menu inicial

    public void Menustart()
    {
        FadeinCanva.SetActive(true);
        if (!isFading)
        {
           // nextSceneName = "Turi";
            StartCoroutine(FadeOutIn());
        }
    }

    //
    IEnumerator FadeOutIn()
    {
        isFading = true;

        // FADE OUT (tela ficando preta)
        float alpha = 0f;
        while (alpha < 1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        // Aqui você pode mudar de cena
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            Destroy(buttonmenu);
        }
            SceneManager.LoadScene(nextSceneName);

        // FADE IN (tela ficando visível)
        alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        isFading = false;
    }
}
