using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class NPC : MonoBehaviour, IObserver
{
    [SerializeField] bool isTalking;
    [SerializeField] string npcName;
    [SerializeField] CinemachineCamera npcCam;

    [Header("Dialogue Parameters")]
    [SerializeField, TextArea (4, 6)] string[] dayDialogueLines;
    [SerializeField, TextArea (4, 6)] string[] nightDialogueLines;
    List <string> dialogueLines = new List<string>();
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMP_Text nameDisplayUI;
    [SerializeField] TMP_Text dialogueTextUI;
    [SerializeField] float secondsBetweenCharacters;

    [SerializeField] bool didDialogueStart;
    bool skippableLine;
    bool lineEnded;
    int lineIndex;
    bool wasDay;

    private void Start()
    {
        wasDay = GameManager.Instance.isDay; 
        if (wasDay)
        {
            for (int i = 0; i < dayDialogueLines.Length; i++)
            {
                dialogueLines.Add(dayDialogueLines[i]);
                //dialogueLines[i] = dayDialogueLines[i];
                //dialogueLines[i] = dayDialogueLines[i];
            }
        }
        else
        {
            for (int i = 0; i < nightDialogueLines.Length; i++)
            {
                dialogueLines.Add(nightDialogueLines[i]);
            }
        }
            
    }
    public void OnNotify()
    {
        PlayerInteraction();
    }
    void PlayerInteraction()
    {
        if(!didDialogueStart)
        {
            StartDialogue();
        }
        else
        {
            if(lineEnded)
            {
                CloseDialogue();
            }
            else
            {
                if(skippableLine)
                {
                    StopAllCoroutines();
                    dialogueTextUI.maxVisibleCharacters = dialogueLines[lineIndex].Length;
                    skippableLine = false;
                    lineEnded = true;
                    Debug.Log("Skipped");
                }
            }
        }
    }
    void CheckDay()
    {
        int slotsToAdd;
        int slotsToRemove;
        if (GameManager.Instance.isDay != wasDay)//esto en teoría solo es si cambia 
        {
            wasDay = !wasDay;
            if (wasDay)
            {
                if (dayDialogueLines.Length > dialogueLines.Count)
                {
                    slotsToAdd = dayDialogueLines.Length - dialogueLines.Count;
                    while (slotsToAdd > 0)
                    {
                        dialogueLines.Add("");
                        slotsToAdd--;
                    }
                }
                else
                {
                    slotsToRemove = dialogueLines.Count - dayDialogueLines.Length;
                    while (slotsToRemove > 0)
                    {
                        dialogueLines.RemoveAt(dialogueLines.Count - 1);
                        slotsToRemove--;
                    }
                }

                for (int i = 0; i < dayDialogueLines.Length; i++)
                {
                    dialogueLines[i] = dayDialogueLines[i];
                }
            }
            else
            {
                if (nightDialogueLines.Length > dialogueLines.Count)
                {
                    slotsToAdd = nightDialogueLines.Length - dialogueLines.Count;
                    while (slotsToAdd > 0)
                    {
                        dialogueLines.Add("");
                        slotsToAdd--;
                    }
                }
                else
                {
                    slotsToRemove = dialogueLines.Count - nightDialogueLines.Length;
                    while (slotsToRemove > 0)
                    {
                        dialogueLines.RemoveAt(dialogueLines.Count - 1);
                        slotsToRemove--;
                    }
                }

                for (int i = 0; i < nightDialogueLines.Length; i++)
                {
                    dialogueLines[i] = nightDialogueLines[i];
                }
            }
        }
    }
    void StartDialogue()
    {
        CheckDay();
        GameManager.Instance.interactMark.SetActive(false);
        didDialogueStart = true;
        GameManager.Instance.playerPaused = true;
        npcCam.Priority = 1;
        GameManager.Instance.playerCam.Priority = 0;
        StartCoroutine(ShowLine());
    }
    private IEnumerator ShowLine()
    {
        dialogueTextUI.text = string.Empty;

        dialogueTextUI.maxVisibleCharacters = 0;
        dialogueTextUI.text = dialogueLines[lineIndex];
        yield return new WaitForSeconds(1.5f);
        nameDisplayUI.text = npcName;
        dialoguePanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        dialogueTextUI.maxVisibleCharacters++;
        yield return new WaitForSeconds(secondsBetweenCharacters);
        skippableLine = true;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueTextUI.maxVisibleCharacters ++;
            yield return new WaitForSeconds(secondsBetweenCharacters);
        }
        yield return new WaitForSeconds(0.5f);
        lineEnded = true;
    }
    void CloseDialogue()
    {
        GameManager.Instance.interactMark.SetActive(true);
            
        didDialogueStart = false;
        dialoguePanel.SetActive(false);
        GameManager.Instance.playerCam.Priority = 1;
        npcCam.Priority = 0;
        if (lineIndex < dialogueLines.Count-1) lineIndex++;
        else
        {
            lineIndex = 0;
        }
        lineEnded = false;
        GameManager.Instance.playerPaused = false;
    }
}

