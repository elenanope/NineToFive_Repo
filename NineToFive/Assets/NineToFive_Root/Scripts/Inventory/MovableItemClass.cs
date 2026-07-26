using UnityEngine;

[CreateAssetMenu (fileName = "new Movable Class", menuName = "Desktop/Item")]
public class MovableItemClass : ItemClass
{
    public bool receives;

    public override ItemClass GetItem() { return this; }
    public override MovableItemClass GetMovable() { return this; }
}
