using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineAutoFollow : MonoBehaviour
{
    private CinemachineCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TrySetFollow();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySetFollow();
    }

    private void TrySetFollow()
    {
        if (vcam == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
        else
        {
            Debug.LogWarning("[CinemachineAutoFollow] Nenhum objeto com tag 'Player' encontrado!");
        }
    }
}