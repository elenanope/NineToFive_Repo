using Unity.Cinemachine;
using UnityEngine;

public class PC : MonoBehaviour, IObserver
{
    [SerializeField] GameObject blackPanel;
    [SerializeField] CinemachineCamera pcCam;
    [SerializeField] GameObject errorsPanel;
    //[SerializeField] TextHandler errorsScript;
    [SerializeField] DesktopManager desktopManager;
    bool isOn= false;
    public void OnNotify()
    {
        GameManager.Instance.interactMark.SetActive(false);
        if (GameManager.Instance.isDay) Switch();
    }
    private void Start()
    {
    }
    private void Update()
    {
        if (GameManager.Instance.errorsToDo)
        {
            desktopManager.enabled = false;
            errorsPanel.SetActive(true);
        }
        else
        {
            if(!desktopManager.enabled) desktopManager.enabled = true;
            else
            {
                //if(desktopManager.) SI SE HA GANADO EL PC; RESTABLECER TODOS LOS MENUS SI QUEDAN TAREAS POR ESE DÏA; SINO; SACAR MENU DE QUE HAS ACABADO EL DIA
            }
        }
        if(!GameManager.Instance.isDay && isOn)
        {
            Switch();
        }
    }
    void Switch()
    {
            isOn = !isOn;
            GameManager.Instance.playerPaused = isOn;
            blackPanel.SetActive(!isOn);
            if (!isOn)
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
