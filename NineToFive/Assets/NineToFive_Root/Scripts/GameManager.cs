using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Subject
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get 
        { 
            if (instance == null)
            {
                Debug.Log("GameManager is null");
            }
            return instance; 
        }
    }

    [Header("Day Statistics")]
    public bool isDay = true;
    public int dayNumber = 1;
    [SerializeField] Color dayColor;//colores de las barras de tiempo
    [SerializeField] Color nightColor;
    [SerializeField] GameObject nightShiftPanel;

    [Header("Work Parameters")]
    public int errorsCleared = 0;
    public int desktopsCleared = 0;
    float timeTotal;
    float timeLeft;
    public float workTimeTotal = 120;
    public float nightTimeTotal = 180;
    public int errorsLeft;
    public int desktopsLeft;

    [Header("Other Parameters")]
    public bool gamePaused = false;
    public bool playerPaused = false;
    public GameObject player;
    public GameObject interactMark;
    public Image timeCountdown;
    public Transform holdingPoint;
    [Header("Camera Shake")]
    public CinemachineCamera playerCam;
    [SerializeField] CinemachineBasicMultiChannelPerlin camNoise;
    [SerializeField] float shakeForce = 18f;
    [SerializeField] float lastingSeconds = 1f;
    float originalForce;
    float shakeCooldown = 3f;//sobretodo por playtest
    float lastShake = 0f;
    bool canShake = true;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        timeLeft = workTimeTotal;
        if (isDay)
        {
            timeCountdown.color = dayColor;
            timeTotal = workTimeTotal;
        }
        else
        {
            timeCountdown.color = nightColor;
            timeTotal = nightTimeTotal;
        }
    }
    private void Update()
    {
        if(!gamePaused)
        {
            if (!canShake)
            {
                if (lastShake < shakeCooldown) lastShake += Time.deltaTime;
                else
                {
                    canShake = true;
                    lastShake = 0f;
                }
            }

            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                timeCountdown.fillAmount = timeLeft / timeTotal;
            }
            else
            {
                timeLeft = 0;
                NotifyObservers();
                TimeChange();
                GamePaused(true);
            }
            //AÑADIR MÄS DÏAS + TRANSICIÖN DE NOCHE A DÏA
        }

    }
    void TimeChange()
    {
        //CHECKEAR SI HAS CUMPLIDO TODAS TUS TAREas DE HOY; SINO; YA HACES ESTO
        isDay = !isDay;
        if(isDay)
        {
            timeTotal = workTimeTotal;
            timeCountdown.color = dayColor;
            //cambiar luz/frases
        }
        else
        {
            timeTotal = nightTimeTotal;
            timeCountdown.color = nightColor;
            nightShiftPanel.SetActive(true);
            //cambiar luz/frases
        }
        timeLeft = timeTotal;
        //sale cartel de que es de noche y si le das a un botón, se llama a GamePaused, se despausa y se cierra el panel
    }
    //si es de noche, la barra se rellena y se pone en otro color + cambia la iluminación

    public void GamePaused(bool pause)
    {
        player.SetActive(!pause);
        gamePaused = pause;
    }

    public void CameraShake()//y que notifique a listeners como el player para que se le caigan cosas o el jefe para su anim
    {
        if(canShake) StartCoroutine(Shaking());
    }
    IEnumerator Shaking()
    {
        canShake = false;
        originalForce = camNoise.FrequencyGain;
        camNoise.FrequencyGain = shakeForce;
        yield return new WaitForSeconds(lastingSeconds);
        camNoise.FrequencyGain = originalForce;
    }
}
