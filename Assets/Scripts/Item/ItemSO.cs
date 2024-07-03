using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmptyItemSO", menuName = "Create Item")]
public class ItemSO : ScriptableObject
{
    public Sprite itemIcon;
    public bool stackable = false;
    public string itemName;

}
