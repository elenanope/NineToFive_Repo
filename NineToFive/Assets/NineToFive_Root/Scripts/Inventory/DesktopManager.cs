using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DesktopManager : MonoBehaviour
{
    [SerializeField] GameObject slotHolder;
    [SerializeField] GameObject cursor;
    //[SerializeField] List<ItemClass> itemsToAdd = new List<ItemClass>();//
    //[SerializeField] List<ItemClass> itemsToRemove = new List<ItemClass>();//

    //[SerializeField] SlotClass[] startingItems;
    [SerializeField] DesktopData dailyData;

    SlotClass[] items;
    SlotClass movingSlot;
    SlotClass originalSlot;
    SlotClass clickedSlot;
    int lastPos;
    GameObject[] slots;
    bool isMovingItem;
    int iconsToDelete;
    int iconsDeleted;
    int day=0;

    private void Awake()
    {
        slots = new GameObject[slotHolder.transform.childCount];
        items = new SlotClass[slots.Length];

        for (int i = 0; i < slotHolder.transform.childCount; i++)
        {
            slots[i] = slotHolder.transform.GetChild(i).gameObject;
        }
        NewDay();
        /*for (int i = 0; i < startingItems.Length; i++)
        {
            items[i] = startingItems[i];
            if(items[i].GetItem() != null)
            {
                if (!items[i].GetItem().isMovable)
                {
                    iconsToDelete++;
                }
            }
            
        }

        RefreshUI();*/
        //foreach (var item in itemsToAdd){Add(item);}
    }
    private void Update()
    {
        cursor.SetActive(isMovingItem);
        cursor.transform.position = Mouse.current.position.ReadValue();
        if (day != GameManager.Instance.dayNumber) NewDay();
    }
    public void RefreshUI()
    {
        for (int i = 0;i < slots.Length;i++)
        {
            try
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                slots[i].transform.GetChild(1).GetComponent<TMP_Text>().enabled = true;
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = items[i].GetItem().icon;
                slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = items[i].GetItem().iconName;
            }
            catch 
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(1).GetComponent<TMP_Text>().text = "";
                slots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                slots[i].transform.GetChild(1).GetComponent<TMP_Text>().enabled = false;
            }
        }
    }

    public void OnClick(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            if(isMovingItem)
            {
                EndItemMove();
            }
            else BeginItemMove();
        }
    }

    private bool BeginItemMove()
    {
        clickedSlot = GetClosestSlot();
        if(clickedSlot == null  || clickedSlot.GetItem()== null) return false;

        movingSlot = new SlotClass(clickedSlot.GetItem());
        originalSlot = new SlotClass(movingSlot.GetItem());
        clickedSlot.Clear();
        isMovingItem = true;
        cursor.GetComponent<Image>().sprite = movingSlot.GetItem().icon;
        RefreshUI();
        return true;
    }
    private bool EndItemMove()
    {
        clickedSlot = GetClosestSlot();
        if(clickedSlot != null && clickedSlot.GetItem()!= null)
        {
            if (clickedSlot.GetItem().iconName == "trash")
            {
                iconsDeleted++;
                if(iconsDeleted == iconsToDelete)
                {
                    GameManager.Instance.desktopsCleared++;
                    //sacar ventana de pc numero X cleared
                }
                //sonido de basura
                //se suma punto adonde sea
            }
            else
            {
                Add(movingSlot.GetItem(), lastPos);
                //lo deja donde estaba al principio
            }
        }
        else
        {
            Add(movingSlot.GetItem(), lastPos);
            //lo deja donde estaba al principio
        }
        lastPos = items.Length - 1;
        movingSlot.Clear();
        isMovingItem = false;
        RefreshUI();
        return true;
    }
    private SlotClass GetClosestSlot()
    {
        for(int i = 0; i< slots.Length; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(slots[i].transform.position);
            if (Vector2.Distance(Mouse.current.position.ReadValue(), screenPoint) < 32)
            {
                if (items[i]!= null)
                {
                    if (items[i].GetItem() != null)
                    {
                        if (items[i].GetItem().isMovable || isMovingItem)
                        {
                            if (!isMovingItem) lastPos = i;
                            return items[i];

                        }
                        else return null;
                    }
                    else return null;
                }
                else return null;
            }
        }
        return null;
    }

    
    public void Add(ItemClass item, int i)
    {
        Debug.Log(item);
        Debug.Log(i);
        items[i].AddItem(item);
        RefreshUI();
    }/*
    public void Remove(SlotClass item)
    {
        items.Remove(item);
        RefreshUI();
    }

    public SlotClass Contains(ItemClass item)
    {
        foreach(SlotClass slot in items)
        {
            if(slot.GetItem() = item) return slot;
        }
        return null;
    }*/
    public void NewDay()
    {
        iconsToDelete = 0;
        iconsDeleted = 0;
        GameManager.Instance.desktopsCleared=0;
        day = GameManager.Instance.dayNumber;
        int dailyLength=0; 
        if (GameManager.Instance.dayNumber == 1) dailyLength = dailyData.day1.Length;
        else if (GameManager.Instance.dayNumber == 2) dailyLength = dailyData.day2.Length;
        else if(GameManager.Instance.dayNumber == 3) dailyLength = dailyData.day3.Length;
        for (int i = 0; i < dailyLength; i++)
        {
                if (GameManager.Instance.dayNumber == 1) items[i] = new SlotClass( dailyData.day1[i].GetItem());
                if (GameManager.Instance.dayNumber == 2) items[i] = new SlotClass(dailyData.day2[i].GetItem());
                if (GameManager.Instance.dayNumber == 3) items[i] = new SlotClass(dailyData.day3[i].GetItem());

                if (items[i].GetItem() != null)
            {
                if (items[i].GetItem().isMovable)
                {
                    iconsToDelete++;
                }
            }

        }

        RefreshUI();
    }
}
