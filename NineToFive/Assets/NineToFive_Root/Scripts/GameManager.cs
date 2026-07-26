using System.Collections;
using TMPro;
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
    [SerializeField] GameObject dayShiftPanel;
    [SerializeField] TMP_Text dayShiftText;
    [SerializeField] TMP_Text errorsTotal;
    [SerializeField] TMP_Text desktopsToDo;
    [SerializeField] TMP_Text papersToDo;
    [SerializeField] GameObject winPanel;

    [Header("Work Parameters")]
    public int errorsCleared = 0;
    public int desktopsCleared = 0;
    public int paperCleared = 0;
    float timeTotal;
    float timeLeft;
    public float workTimeTotal = 120;
    public float nightTimeTotal = 180;
    public bool errorsToDo;
    [SerializeField] DesktopData dailyData;
    bool dayFulfilled;

    [Header("Other Parameters")]
    public bool gamePaused = false;
    public bool playerPaused = false;
    public GameObject player;
    public GameObject interactMark;
    public Image timeCountdown;
    public Transform holdingPoint;
    public bool beingCarried;
    public GameObject objectCarried;
    public Animator playerAnim;

    [Header("Camera Shake")]
    public CinemachineCamera playerCam;
    [SerializeField] CinemachineBasicMultiChannelPerlin camNoise;
    [SerializeField] float shakeForce = 18f;
    [SerializeField] float lastingSeconds = 1f;
    float originalForce;
    float shakeCooldownMin = 7f;//sobretodo por playtest
    float shakeCooldownMax = 17f;//sobretodo por playtest
    float nextShake;//sobretodo por playtest
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
            if(beingCarried && !playerAnim.GetBool("carriesObject"))
            {
                playerAnim.SetBool("carriesObject", true);
            }
            else if(!beingCarried && playerAnim.GetBool("carriesObject"))
            {
                playerAnim.SetBool("carriesObject", false);
            }
            if(!isDay)
            {

                papersToDo.text = "paper:" + paperCleared + "/" + dailyData.paperNeeded[dayNumber - 1];
                if (paperCleared>= dailyData.paperNeeded[dayNumber - 1])TimeChange();
                if (!canShake)
                {
                    if(nextShake == 0f)
                    {
                        nextShake = Random.Range(shakeCooldownMin, shakeCooldownMax);
                    }
                    else
                    {
                        if (lastShake < nextShake) lastShake += Time.deltaTime;
                        else
                        {
                            canShake = true;
                            lastShake = 0f;
                        }
                    }
                }
                else
                {
                    canShake = false;
                    CameraShake();
                }
            }
            else
            {
                errorsTotal.text = "errors:"+ errorsCleared + "/" + dailyData.errorsNeeded[dayNumber-1];
                desktopsToDo.text = "desktop:" + desktopsCleared + "/" + dailyData.desktopsNeeded[dayNumber-1];
            }

            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                timeCountdown.fillAmount = timeLeft / timeTotal;
            }
            else
            {
                timeLeft = 0;
                //NotifyObservers();
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
            if(dayNumber<3)
            {
                dayNumber++;
                timeTotal = workTimeTotal;
                timeCountdown.color = dayColor;
                dayShiftPanel.SetActive(true);
                dayShiftText.text = "Day "+ dayNumber;
                errorsTotal.gameObject.SetActive(true);
                desktopsToDo.gameObject.SetActive(true);
                papersToDo.gameObject.SetActive(false);
                //cambiar luz/frases
            }
            else
            {
                winPanel.SetActive(true);
                //congratulations!!! you overcame your first week!
            }

        }
        else
        {
            CheckFulfillment();
            
            //cambiar luz/frases
        }
        timeLeft = timeTotal;
        //sale cartel de que es de noche y si le das a un botón, se llama a GamePaused, se despausa y se cierra el panel
    }
    //si es de noche, la barra se rellena y se pone en otro color + cambia la iluminación

    void CheckFulfillment()
    {
        if(errorsCleared >= dailyData.errorsNeeded[dayNumber-1] && desktopsCleared >= dailyData.desktopsNeeded[dayNumber-1])
        {
            Debug.Log("Well done");
            dayFulfilled = true;
            TimeChange();
        }
        else
        {
            dayFulfilled = false; 
            timeTotal = nightTimeTotal;
            timeCountdown.color = nightColor;
            nightShiftPanel.SetActive(true);
            errorsTotal.gameObject.SetActive(false);
            desktopsToDo.gameObject.SetActive(false);
            papersToDo.gameObject.SetActive(true);
            if (dayNumber == 3)
            {
                shakeCooldownMin = 4f;
                shakeCooldownMax = 10f;
            }
        }
    }
    public void GamePaused(bool pause)
    {
        player.SetActive(!pause);
        gamePaused = pause;
    }

    public void CameraShake()//y que notifique a listeners como el player para que se le caigan cosas o el jefe para su anim
    {
        StartCoroutine(Shaking());

        NotifyObservers();
    }
    IEnumerator Shaking()
    {
        originalForce = camNoise.FrequencyGain;
        camNoise.FrequencyGain = shakeForce;
        yield return new WaitForSeconds(lastingSeconds);
        camNoise.FrequencyGain = originalForce;
    }
}
