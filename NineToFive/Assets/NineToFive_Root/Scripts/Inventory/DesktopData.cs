using UnityEngine;

[CreateAssetMenu(fileName = "new data", menuName = "Desktop/Data")]
public  class DesktopData : ScriptableObject
{
    public SlotClass[] day1;
    public SlotClass[] day2;
    public SlotClass[] day3;

    public int[] errorsNeeded = {2, 3,5};
    public int[] desktopsNeeded = { 1,1,1};
    public int[] paperNeeded = {3,6,12};
}
