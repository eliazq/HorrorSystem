using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquippable
{
    bool IsEquipped { get; set; }
    void Equip();
}
