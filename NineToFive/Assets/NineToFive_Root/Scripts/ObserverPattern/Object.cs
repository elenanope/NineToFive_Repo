using UnityEngine;

public class Object : MonoBehaviour, IObserver
{
    [SerializeField] bool isPickable;
    public void OnNotify()
    {
        GameManager.Instance.interactMark.SetActive(false);
        PlayerInteraction();
    }

    void PlayerInteraction()
    {
        if(isPickable)
        {
            Debug.Log("Objeto cogible!");
            //se coge el objeto
        }
        else
        {
            Debug.Log("Objeto interactuable!");
            //se interactúa
        }
        /*if (interactableObjectInRange)
            {
                Debug.Log("Objeto Interactuable");
                //Lanzar llamada a través de un collider en una sphere pra interactuar con lo que tengas cerca (cambiar el input y etc.)
            }
            if (pickableObjectInRange)
            {
                Debug.Log("Objeto Cogible");
                if (objectToHold != null)
                {
                    if (heldObjectMesh == null)
                    {
                        heldObjectMesh = objectToHold;
                        heldObjectMesh.transform.parent = holdingPoint;
                        heldObjectMesh.transform.localPosition = new Vector3(0f, 0f, 0f);
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
            }*/
    }
}
