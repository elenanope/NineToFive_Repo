using UnityEngine;

[System.Serializable]
public class SlotClass
{
    [SerializeField]ItemClass item;

    public SlotClass(ItemClass _item)
    {
        item = _item;
    }
    public void Clear()
    {
        this.item = null;
    }
    public ItemClass GetItem() { return item; }
    public void AddItem(ItemClass item)
    {
        this.item = item;
    }
}
