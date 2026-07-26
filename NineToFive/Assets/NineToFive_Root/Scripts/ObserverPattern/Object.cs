using UnityEngine;

public class Object : MonoBehaviour, IObserver
{
    public bool isPickable;
    [SerializeField] bool beingCarried;
    public bool canReceive;
    [SerializeField] Collider col;
    [SerializeField] Collider triggerCol;
    [SerializeField] Renderer rend;
    [SerializeField] Rigidbody rb;
    [SerializeField] Subject playerSubject;
    Vector3 lastPos;
    bool dayChange;
    Vector3 spot;
    [SerializeField]bool trashNear;
    public void OnNotify()
    {
        if (!GameManager.Instance.isDay)
        {
            GameManager.Instance.interactMark.SetActive(false);
            PlayerInteraction();
        }
            
    }
    private void Update()
    {
        if (isPickable && !canReceive)
        {
            
            if (GameManager.Instance.isDay && rend.enabled)
            {
                lastPos = transform.position;
                LeaveObject();
                rend.enabled = false;
                rb.useGravity = false;
                triggerCol.enabled = false;
                col.enabled = false;
                transform.position = new Vector3 (transform.position.x, transform.position.y+1.5f, transform.position.z);
            }
                else if (!GameManager.Instance.isDay && !rend.enabled)
            {

                rend.enabled = true;
                triggerCol.enabled = true;
                col.enabled = true;
                rb.useGravity = true;
                transform.position = lastPos;
            }
                
        }
    }
    void PlayerInteraction()
    {
        if(isPickable)
        {
            if(!beingCarried)
            {
                beingCarried = true; 
                GameManager.Instance.beingCarried = true;
                GameManager.Instance.objectCarried = gameObject;
                transform.parent = GameManager.Instance.holdingPoint;
                col.enabled = false;
                rb.useGravity = false;
                transform.localPosition = new Vector3(0f, 0f, 0f);
            }
            else
            {
                if (trashNear)
                {
                    if (GameManager.Instance.beingCarried)
                    {
                        triggerCol.enabled = false;
                        playerSubject.RemoveObserver(this);
                        GameManager.Instance.paperCleared++;
                        Debug.Log("Has reciclado!!!");
                        LeaveObject();
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    LeaveObject();
                    col.enabled = true;
                    rb.useGravity = true;
                }
               
            }
            //se coge el objeto
        }
        else
        {
            
            //se interactúa
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<Object>(out Object obj))
        {
            if(!obj.isPickable && obj.canReceive)
            {
                trashNear = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent<Object>(out Object obj))
        {
            trashNear = false;
        }
    }
    void LeaveObject()
    {
        beingCarried = false;
        GameManager.Instance.beingCarried = false;
        GameManager.Instance.objectCarried = null;
        transform.parent = null;
        if(beingCarried)
        {
            spot = new Vector3(GameManager.Instance.player.transform.position.x + 1f, GameManager.Instance.player.transform.position.y + 0.4f, GameManager.Instance.player.transform.position.z + 1f);
            transform.position = spot;
        }
           
    }
}
