using Unity.Cinemachine;
using UnityEngine;

public class PC : MonoBehaviour, IObserver
{
    [SerializeField] GameObject blackPanel;
    [SerializeField] CinemachineCamera pcCam;
    bool isOn= false;
    public void OnNotify()
    {
        GameManager.Instance.interactMark.SetActive(false);
        Switch();
    }

    void Switch()
    {
        isOn = !isOn;
        GameManager.Instance.playerPaused = isOn;
        blackPanel.SetActive(!isOn);
        if(!isOn)
        {
            pcCam.Priority = 0;
            GameManager.Instance.playerCam.Priority = 1;
        }
        else
        {
            pcCam.Priority = 1;
            GameManager.Instance.playerCam.Priority = 0;
        }
        //camera switch
        //quitar Inputs
        //quitarPanel
    }
}
