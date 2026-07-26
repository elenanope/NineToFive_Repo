using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController :Subject, IObserver
{
    [SerializeField] float movementSpeed;
    public bool carriesObject;
    [SerializeField] float rotationTime;
    float timeSinceMove;
    [SerializeField] float maxForce;
    Vector2 moveInput;
    bool interacting;
    bool canInteract = true;
    bool objectInRange;
    bool npcInRange;
    [SerializeField] float interactingCooldown;
    [SerializeField] Rigidbody playerRb;
    [SerializeField] GameObject objectNear;
    [SerializeField] GameObject npcNear;
    [SerializeField] Animator playerAnim;

    [SerializeField] Subject _gameManagerSubject;
    private void Start()
    {
        playerAnim = GameManager.Instance.playerAnim;
    }
    private void OnEnable()
    {
        _gameManagerSubject.AddObserver(this);
    }
    private void OnDisable()
    {
        _gameManagerSubject.RemoveObserver(this);
    }

    public void OnNotify()
    {
        if(GameManager.Instance.beingCarried)
        {
            //se te cae el objeto
            NotifyObservers();
        }
        Debug.Log("Temblor!");
    }

    private void Update()
    {
        if (interacting) StartCoroutine(InteractRoutine());

        timeSinceMove += Time.deltaTime;

        if (timeSinceMove >= 20f)
        {
            //anim.SetTrigger("varyIdle");
            timeSinceMove = -10;
        }
    }
    private void FixedUpdate()
    {
        if (!GameManager.Instance.playerPaused)
        {
            Move();
        }
    }

    void Interact()
    {
        if (objectInRange || npcInRange)
        {
            //if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);
            //playerSpeaker.Stop();
            playerRb.linearVelocity = Vector3.zero;
            NotifyObservers();
        }
    }
    IEnumerator InteractRoutine()
    {
        interacting = false;
        if (canInteract) Interact();
        canInteract = false;
        yield return new WaitForSeconds(interactingCooldown);
        canInteract = true;
    }
    void Move()
    {
        Vector3 forward = Vector3.forward;
        Vector3 right = Vector3.right;

        forward.y = 0;
        forward.Normalize();
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationTime * Time.deltaTime);
        }

        Vector3 currentVelocity = playerRb.linearVelocity;
        Vector3 targetVelocity = moveDirection;
        targetVelocity *= movementSpeed;

        // Calcular el cambio de velocidad (aceleración)
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3(velocityChange.x, 0, velocityChange.z);
        velocityChange = Vector3.ClampMagnitude(velocityChange, maxForce);
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            timeSinceMove = 0;
            if (!playerAnim.GetBool("isWalking")) playerAnim.SetBool("isWalking", true);
            //if (!playerSpeaker.isPlaying) playerSpeaker.Play();
        }
        else
        {
            if (playerAnim.GetBool("isWalking"))
            {
                timeSinceMove = 0;
                playerAnim.SetBool("isWalking", false);
            }

            //if (playerSpeaker.isPlaying) playerSpeaker.Stop();
        }
        playerRb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    private void OnTriggerStay(Collider other)
    {
        if (!objectInRange)
        {
            if (other.gameObject.TryGetComponent<Object>(out Object soulless))
            {
                objectInRange = true;
                objectNear = other.gameObject;
                this.AddObserver(soulless);
                GameManager.Instance.interactMark.SetActive(true);
            }
            if (other.gameObject.TryGetComponent<PC>(out PC pc) && GameManager.Instance.isDay)
            {
                objectInRange = true;
                objectNear = other.gameObject;
                this.AddObserver(pc);
                GameManager.Instance.interactMark.SetActive(true);
            }
        }
        if(!npcInRange)
        {
            if (other.gameObject.TryGetComponent<NPC>(out NPC soul))
            {
                npcInRange = true;
                npcNear = other.gameObject;
                this.AddObserver(soul);
                GameManager.Instance.interactMark.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Object>(out Object soulless))
        {
            objectInRange = false;
            this.RemoveObserver(soulless);
            objectNear = null;
            GameManager.Instance.interactMark.SetActive(false);
        }
        if (other.gameObject.TryGetComponent<PC>(out PC pc))
        {
            objectInRange = false;
            this.RemoveObserver(pc); 
            objectNear = null;
            GameManager.Instance.interactMark.SetActive(false);
        }
        if (other.gameObject.TryGetComponent<NPC>(out NPC soul))
        {
            npcInRange = false;
            this.RemoveObserver(soul);
            npcNear = null;
            GameManager.Instance.interactMark.SetActive(false);
        }
    }
    //Movement
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        //checkear si es dentro/fuera del pc
        if (ctx.performed) interacting = true;
    }
    //Interaction (Simple + Grabbing)

}
