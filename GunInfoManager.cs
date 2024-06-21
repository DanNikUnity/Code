using BigRookGames.Weapons;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunInfoManager : MonoBehaviour
{

    public Shoot Gun;
    public string GunName;

    public int index;

    [Header("Laser")]

    public GameObject Laser;
    public bool hasLaser;
    public bool isEquipedLaser;
    public int laserPrice;

    [Header("Silencer")]

    public GameObject Silencer;
    public bool hasSilencer;
    public int silencerPrice;
    public bool isEquipedSilencer;

    [Header("ExtMagazine")]

    public bool hasExtMagazine;
    public int extMagazinePrice;
    public bool isEquipedExtMagazine;


    public bool isBought; 
    public int gunPrice;

    public int magazineCount;
    public int magazinePrice;

    public Sprite icon;

    //stats;
    public float FireRate;
    public float StoreCapacity;
    public float Accuracy;
    public float Reload;
    public float Secretiveness;

    public string description;

    public float initialRadius;

    [HideInInspector]
    public float initialDeviation;
    [HideInInspector]
    public float initialReloadTime;
    [HideInInspector]
    public float initialMaxMagazine;
    [HideInInspector]
    public float initialMoveSpeed;
    [HideInInspector]

    private SoundManager sound;
    private void Awake()
    {
        if (Gun != null)
        {
            //FireRate = Gun.FireRate*60;
            StoreCapacity = Gun.MagazineMax;
            Accuracy = Gun.maxDeviationAngle;
            Reload = Gun.MagazineReload;

            //initialRadius = HCol.radius;
            if (Gun.isShotgun) initialDeviation = Gun.spreadAngle;
            else initialDeviation = Gun.maxDeviationAngle;
            initialReloadTime = Gun.MagazineReload;
            initialMaxMagazine = Gun.MagazineMax;
            initialMoveSpeed = 6f;
        }
        gameObject.SetActive(false);
        sound = FindObjectOfType<SoundManager>();
    }
    private void Update()
    {
        if (Gun != null)
        {
            magazineCount = Gun.magazineCount;
        }
        else
        {
            GunfireController gfc = GetComponent<GunfireController>();
            magazineCount = gfc.Ammo;
        }
    }
    public void ToggleLaser()
    {
        if(hasLaser && laserPrice !=0)
        {
            if(isEquipedLaser)
            {
                Laser.SetActive(false);
                isEquipedLaser = false;

                if (Gun == null)
                {
                    GunfireController GFC = GetComponent<GunfireController>();
                    if (GFC != null)
                    {
                        GFC.maxDeviationAngle = GFC.maxDeviationAngle * 2;
                        SphereCollider PHCol = GameObject.FindGameObjectWithTag("HearCol").GetComponent<SphereCollider>();
                      //  PHCol.radius = PHCol.radius - 4;
                    }
                }
                else
                {
                    Gun.maxDeviationAngle = Gun.maxDeviationAngle * 2;
                    SphereCollider HCol = GameObject.FindGameObjectWithTag("HearCol").GetComponent<SphereCollider>();
                   // HCol.radius = HCol.radius - 4;
                }
            }
            else
            {
                Laser.SetActive(true);
                isEquipedLaser = true;

                if (Gun == null)
                {
                    GunfireController GFC = GetComponent<GunfireController>();
                    if (GFC != null)
                    {
                        GFC.maxDeviationAngle = GFC.maxDeviationAngle * 0.5f;
                        SphereCollider PHCol = GameObject.FindGameObjectWithTag("HearCol").GetComponent<SphereCollider>();
                      //  PHCol.radius = PHCol.radius + 4;
                    }
                }
                else
                {
                    Gun.maxDeviationAngle = Gun.maxDeviationAngle * 0.5f;
                    SphereCollider HCol = GameObject.FindGameObjectWithTag("HearCol").GetComponent<SphereCollider>();
                   // HCol.radius = HCol.radius + 4;
                }
            }
            CalculateValues();
            sound.Click();
        }
        else sound.NoMoney();
    }
    public void ToggleSilencer()
    {
        if (hasSilencer && silencerPrice != 0)
        {
            if (isEquipedSilencer)
            {
                // Silencer.SetActive(false);
                isEquipedSilencer = false;

                Gun.maxDeviationAngle = Gun.maxDeviationAngle * 2;
                SphereCollider HCol = GameObject.FindGameObjectWithTag("HearCol").GetComponent<SphereCollider>();
                //    HCol.radius = 5f;
                //   if (isEquipedLaser) HCol.radius = 7.5f;

            }
            else
            {
                // Silencer.SetActive(true);
                isEquipedSilencer = true;
            }
            CalculateValues();
            sound.Click();
        }
        else sound.NoMoney();
    }
    public void ToggleExtMagazine()
    {
        if(hasExtMagazine && extMagazinePrice != 0)
        {
            isEquipedExtMagazine = !isEquipedExtMagazine;
            CalculateValues();
            sound.Click();
        }
        else sound.NoMoney();
    }

    public void CalculateValues()
    {
        if (Gun != null)
        {
            //скрытность
            float RadBuff = 0f;
            if (hasSilencer && isEquipedSilencer) RadBuff -= 75f;
            if (hasLaser && isEquipedLaser) RadBuff += 25f;
            SphereCollider HCol = GameObject.FindGameObjectWithTag("HearCol").GetComponent<SphereCollider>();
            HCol.radius = initialRadius * (RadBuff * 0.01f) + initialRadius;
            Secretiveness = HCol.radius;

            // Точность
            float Deviation = 0f;
            if (hasLaser && isEquipedLaser) Deviation -= 50f;
            if (hasSilencer && isEquipedSilencer) Deviation += 20;
            if (Gun.isShotgun)
            {
                Gun.spreadAngle = initialDeviation + initialDeviation * (Deviation * 0.01f);
                Accuracy = Gun.spreadAngle;
            }
            else
            {
                Gun.maxDeviationAngle = initialDeviation + initialDeviation * (Deviation * 0.01f);
                Accuracy = Gun.maxDeviationAngle;
            }



            //обойма
            float Magazine = 0f;
            if (hasExtMagazine && isEquipedExtMagazine) Magazine += 50f;
            float reload = 0f;
            if (hasExtMagazine && isEquipedExtMagazine) reload += 50f;
            Gun.MagazineMax = initialMaxMagazine + initialMaxMagazine * Magazine * 0.01f;
            Gun.MagazineReload = initialReloadTime + initialReloadTime * reload * 0.01f;
            StoreCapacity = Gun.MagazineMax;
            Reload = Gun.MagazineReload;
        }
        else
        {
            SphereCollider HCol = GameObject.FindGameObjectWithTag("HearCol").GetComponent<SphereCollider>();
            HCol.radius = initialRadius;
            Secretiveness = HCol.radius;
        }
    }
}
