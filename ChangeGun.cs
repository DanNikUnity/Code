using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGun : MonoBehaviour
{
    private int prevIndex = -1;
    public InfoToUI ITU;
    private bool canSwitch = true;
    public void EquipGun(int index)
    {
        
        if (ITU.GunInfo[index].isBought && index != prevIndex)
        {
            if (!canSwitch) return;
            canSwitch = false;
            ITU.GunInfo[index].gameObject.SetActive(true);
            if(prevIndex >=0) ITU.GunInfo[prevIndex].gameObject.SetActive(false);
            Invoke("EnableIt", 1.1f);
            ITU.Guns[index].GetComponentInChildren<Canvas>(true).gameObject.SetActive(true);
            if(prevIndex >=0) ITU.Guns[prevIndex].GetComponentInChildren<Canvas>(true).gameObject.SetActive(false);
            prevIndex = index;

        }
        
    }
    void EnableIt()
    {
        canSwitch = true;
    }
    public void Start()
    {
        EquipGun(0);
    }
    public void EquipRocketer()
    {
        ITU.GunInfo[prevIndex].gameObject.SetActive(false);
    }
}
