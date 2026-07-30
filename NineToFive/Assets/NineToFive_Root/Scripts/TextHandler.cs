using System.Collections;
using TMPro;
using UnityEngine;

public class TextHandler : Subject
{
    [Header ("References and Info")]
    [SerializeField] TMP_Text[] randomLettersUI;
    [SerializeField] TMP_Text[] randomNumbersUI;//el objeto que contenga este componente también tendrá uno de botón
    [SerializeField] int totalLettersPerColumn;
    [SerializeField] GameObject numbersPanel;
    string[] randomLetters;
    string letters = "abcdefghjklmnpqrstuvwxyz";
    [SerializeField]int totalNumbers;
    [SerializeField] int numbersCatched = 0;
    bool lettersReady;
    bool numbersReady;
    [SerializeField] int times= 1;
    int day= 1;
    private void Start()
    {
        totalNumbers = randomNumbersUI.Length;
        CreateRandomNumbers();
        CreateRandomLetters();
    }
    private void Update()
    {

        if (day != GameManager.Instance.dayNumber)
        {
            day = GameManager.Instance.dayNumber;
            GameManager.Instance.errorsCleared = 0;
            NewErrors();
        }
    }
    private void FixedUpdate()
    {
        if(numbersReady && lettersReady)
        {
            GameManager.Instance.errorsToDo = true;
            NotifyObservers();//HACERLO ASYNC EN EL START?!
        }
    }
    void CreateRandomLetters()
    {
        int randomLetter;
        int firstLetters=0;
        int lettersPerColumn;
        bool first = true;//hay solo 2 huecos de palabras en cada columna
        for (int i = 0; i < randomLettersUI.Length; i++)
        {
            lettersPerColumn = totalLettersPerColumn;
            randomLettersUI[i].text = "";
            if (first)
            {
                firstLetters = Random.Range(0, lettersPerColumn - 2);
                if(firstLetters > 0)
                {
                    while (randomLettersUI[i].text.Length < firstLetters)
                    {
                        randomLetter = Random.Range(0, letters.Length);
                        randomLettersUI[i].text += letters.Substring(randomLetter, 1);
                    }
                }
                first = false;
            }
            else
            {
                lettersPerColumn -= firstLetters;
                if (lettersPerColumn > 0)
                {
                    while (randomLettersUI[i].text.Length < lettersPerColumn)
                    {
                        randomLetter = Random.Range(0, letters.Length);
                        randomLettersUI[i].text += letters.Substring(randomLetter, 1);
                    }
                }
                first = true;
            }
        }
        lettersReady = true;
    }
    void CreateRandomNumbers()
    {
        int randomNumber;
        for (int i = 0; i < randomNumbersUI.Length; i++)
        {
            randomNumbersUI[i].gameObject.SetActive(true);
            randomNumber = Random.Range(0, 10);
            randomNumbersUI[i].text = randomNumber.ToString();
        }
        numbersReady = true;
    }
    public void CatchedNumber()
    {
        numbersCatched++;
        if(totalNumbers == numbersCatched)
        {
            CloseWindow();
        }
            
    }
    void CloseWindow()
    {
        GameManager.Instance.errorsCleared++;
        numbersPanel.SetActive(false);
        numbersReady = false;
        lettersReady = false;
        GameManager.Instance.errorsToDo = false;
        numbersCatched = 0;
    }

    public void NewErrors()
    {
        numbersCatched = 0;
        CreateRandomNumbers();
        CreateRandomLetters();
        numbersPanel.SetActive(true);
    }
}
