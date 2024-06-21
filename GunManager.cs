using BigRookGames.Weapons;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] private List<Shoot> shootScripts = new List<Shoot>();
    [SerializeField] private GunfireController GFC;
    [SerializeField] private HearShoootings HS;
    private SoundManager sound;

    /* void Start()
     {
         GameObject[] shootObjects = GameObject.FindGameObjectsWithTag("Gun");
         foreach (GameObject obj in shootObjects)
         {
             Shoot shootScript = obj.GetComponent<Shoot>();
             if (shootScript != null)
             {
                 shootScripts.Add(shootScript);
             }
         }
     }*/

    private void Start()
    {
        sound = GameObject.FindGameObjectWithTag("Manager").GetComponent<SoundManager>();
    }

    public void TryToShootActiveObject()
    {
        foreach (Shoot script in shootScripts)
        {
            if (script.isActiveAndEnabled)
            {
                script.TryToShoot();
                return;
            }
        }
        GFC.TryToShot();
        HS.SetSawPlayerForAll();
    }
    public void BuyAmmo()
    {
        foreach (Shoot script in shootScripts)
        {
            if (script.isActiveAndEnabled && MoneySystem.money >= script.GetComponent<GunInfoManager>().magazinePrice)
            {
                MoneySystem.money -= script.GetComponent<GunInfoManager>().magazinePrice;
                TMP_Text tMP_Text = FindObjectOfType<MarketManager>().MagazineCount;
                script.BuyMagazine();
                sound.OnBought();
                tMP_Text.text = script.magazineCount.ToString();
                return;

            }
            else if (script.isActiveAndEnabled) sound.NoMoney();


        }
        if (GFC.isActiveAndEnabled && MoneySystem.money >= GFC.GetComponent<GunInfoManager>().magazinePrice)
        {
            GFC.BuyAmmo();
            MoneySystem.money -= GFC.GetComponent<GunInfoManager>().magazinePrice;
            sound.OnBought();
        }
        else sound.NoMoney();

    }
   /* public void Reload()
    {
        foreach (Shoot script in shootScripts)
        {
            if (script.isActiveAndEnabled)
            {
                script.Reload();
                return;
            }
        }
    }*/
}
