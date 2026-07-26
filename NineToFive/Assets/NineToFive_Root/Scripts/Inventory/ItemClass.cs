using UnityEngine;

[System.Serializable]
public abstract class ItemClass: ScriptableObject
{
    public string iconName;
    public Sprite icon;
    public bool isMovable;

    public abstract ItemClass GetItem();
    public abstract MovableItemClass GetMovable();
}
