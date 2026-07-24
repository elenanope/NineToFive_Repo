using TMPro;
using UnityEngine;

public class TextPanelMovement : MonoBehaviour, IObserver
{
    bool canMove;
    [SerializeField] Subject textHandler;
    [SerializeField] TMP_Text lowestChild;
    [SerializeField] TMP_Text firstChild;

    [Header("Characters Movement")]
    [SerializeField] float columnSpeed;
    float columnLength;
    float columnStartPos;
    [SerializeField] GameObject cameraUI;
    private void Start()
    {
        textHandler.AddObserver(this);
    }
    public void OnNotify()
    {
        canMove = true;
    }
    private void FixedUpdate()
    {
        if (canMove)
        {
            columnStartPos = lowestChild.bounds.max.y;
            columnLength = lowestChild.bounds.size.y;
            //AnimateLetters();
        }
    }
    void AnimateLetters()
    {

        transform.position = new Vector3(transform.position.x, transform.position.y + columnSpeed, transform.position.z);

        float temp = (transform.position.y * columnSpeed);
        if (temp > columnStartPos + columnLength) columnStartPos += columnLength;
        else if (temp < columnStartPos - columnLength) columnStartPos -= columnLength;
        if(gameObject.GetComponent<RectTransform>().position.y > 800) transform.position = new Vector3(transform.position.x, transform.position.y -1600f, transform.position.z);
    }
}
