using BigRookGames.Weapons;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AmmoToScreen : MonoBehaviour
{
    private Shoot shootScript;
    private GunfireController GFC;
    public TMP_Text MagCount;
    public Image CurentAmmo;

    private void Start()
    {
        shootScript = GetComponent<Shoot>();
        if (shootScript == null)
            GFC = GetComponent<GunfireController>();
    }
    void Update()
    {
        if (shootScript != null && MagCount != null)
        {
            float f = shootScript.currentMagazine / shootScript.MagazineMax;
            MagCount.text = "X" + shootScript.magazineCount.ToString();
            CurentAmmo.fillAmount = f;
        }
        if(GFC != null && MagCount != null)
        {
            float f = GFC.Ammo / 1;
            MagCount.text = "X" + GFC.Ammo.ToString();
            CurentAmmo.fillAmount = f;
        }
    }
}
