using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    public bool carriesObject;
    [SerializeField] float rotationTime;
    float timeSinceMove;
    [SerializeField] float maxForce;
    Vector2 moveInput;
    bool hasTurned;
    bool interacting;
    bool canInteract;
    [SerializeField] float interactingCooldown;
    [SerializeField] Rigidbody playerRb;

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
        //if (!GameManager.Instance.playerInDialogue)
        {
            Move();
        }
    }
    /*void Interact()
    {
        if (canInteract)
        {
            if (npcInRange)
            {
                DialogueManager.Instance.RegisterInfo(dialogueInfo);
                if (anim.GetBool("isWalking")) anim.SetBool("isWalking", false);
                playerSpeaker.Stop();
                playerRb.linearVelocity = Vector3.zero;
                if (objectToHold != null)
                {
                    if (GameManager.Instance.heldObjectMesh == null)
                    {
                        GameManager.Instance.heldObjectMesh = objectToHold;
                        GameManager.Instance.heldObjectMesh.transform.parent = holdingPoint;
                        GameManager.Instance.heldObjectMesh.transform.localPosition = new Vector3(0f, 0f, 0f);
                        DialogueManager.Instance.dialogueMark.SetActive(false);
                        objectToHold = null;
                    }
                    else
                    {
                        //sonido de wrong
                    }
                }
                DialogueManager.Instance.DialogueCall();
                if (!GameManager.Instance.playerInDialogue) npcInRange = false;
            }
            else
            {
                if (GameManager.Instance.heldObjectMesh != null)
                {
                    GameManager.Instance.heldObjectMesh.transform.parent = null;
                    GameManager.Instance.heldObjectMesh.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f);
                    GameManager.Instance.heldObjectMesh = null;
                    GameManager.Instance.heldObject = "";
                }
            }
        }
    }*/
    IEnumerator InteractRoutine()
    {
        interacting = false;
        //if (canInteract) Interact();
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
        if (!hasTurned && moveDirection.sqrMagnitude > 0.001f)
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
            //if (!anim.GetBool("isWalking")) anim.SetBool("isWalking", true);
            //if (!playerSpeaker.isPlaying) playerSpeaker.Play();
        }
        else
        {
            /*if (anim.GetBool("isWalking"))
            {
                timeSinceMove = 0;
                anim.SetBool("isWalking", false);
            }*/

            //if (playerSpeaker.isPlaying) playerSpeaker.Stop();
        }
        playerRb.AddForce(velocityChange, ForceMode.VelocityChange);
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
