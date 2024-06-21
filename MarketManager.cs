using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BigRookGames.Weapons;

public class MarketManager : MonoBehaviour
{

    public InfoToUI G;
    public TMP_Text Name;
    public TMP_Text Description;
    public TMP_Text Stats;
    [Header("Gun")]
    public TMP_Text BuyText;
    public Image GunIcon;
    public Image Gimg;
    public Image GunImage;
    [Header("Laser")]
    public TMP_Text LaserText;
    public Image Laser;
    public Image Limg;
    [Header("Magazine")]
    public TMP_Text MagazineText;
    public Image Magazine;
    public Image Mimg;
    [Header("Silencer")]
    public TMP_Text SilencerText;
    public Image Silencer;
    public Image Simg;
    [Header("Flash")]
    private FlashThrow flashThrow;
    public TMP_Text FlashPrice;
    public TMP_Text FlashAmount;

    [Header("Other")]
    public TMP_Text ammoPrice;
    public TMP_Text MagazineCount;

    public int ix;

    [Header("Stats")]
    public GridObjects Accuracy;
    public GridObjects FireRate;
    public GridObjects MagazineCapacity;
    public GridObjects ReloadSpeed;
    public GridObjects Secretiveness;

    private SoundManager sound;

    public void Start()
    {
        flashThrow = FindObjectOfType<FlashThrow>();
        sound = FindObjectOfType<SoundManager>();
        FlashPrice.text = flashThrow.Price.ToString();
    }
    public void FixedUpdate()
    {
        FlashAmount.text = flashThrow.Amount.ToString();
    }
    private void OnEnable()
    {
        Shoot gun = FindObjectOfType<Shoot>();
        if(gun != null)
        {
            ammoPrice.text = gun.GetComponent<GunInfoManager>().magazinePrice.ToString();
            MagazineCount.text = gun.magazineCount.ToString();
        }
        else
        {
            GunfireController gfc = FindObjectOfType<GunfireController>();
            ammoPrice.text = gfc.GetComponent<GunInfoManager>().magazinePrice.ToString();
            MagazineCount.text = gfc.GetComponent<GunInfoManager>().magazineCount.ToString();
        }

    }
    public void LoadGunDates(int index)
    {
        ix = index;
        GunInfoManager M = G.GunInfo[index];
        Name.text = M.GunName;
        M.CalculateValues();
        Description.text = M.description;


        if (M.gunPrice != 0 ) BuyText.text = "Buy Weapon" + "\n" + M.gunPrice.ToString();
        if (M.isBought || M.gunPrice == 0)
        {
            ChangeColorToGray(GunImage);
            Gimg.gameObject.SetActive(true);
        }
        else
        {
            ChangeToTransparent(GunImage);
            Gimg.gameObject.SetActive(false);
        }


        if (M.hasSilencer) Simg.gameObject.SetActive(true);
        else Simg.gameObject.SetActive(false);
        if (M.silencerPrice == 0 || M.hasSilencer)
        {
            ChangeColorToGray(Silencer);
            SilencerText.text = null;
        }
        else if (M.silencerPrice != 0 && !M.hasSilencer) 
        {
            SilencerText.text = "Buy Silencer" + "\n" + M.gunPrice.ToString();
            if (M.isBought) ChangeToTransparent(Silencer);
            else ChangeColorToGray(Silencer);
        }


        if (M.hasExtMagazine) Mimg.gameObject.SetActive(true);
        else Mimg.gameObject.SetActive(false);
        if (M.extMagazinePrice == 0 || M.hasExtMagazine)
        {
            ChangeColorToGray(Magazine);
            MagazineText.text = null;
        }
        else if (M.extMagazinePrice != 0 && !M.hasExtMagazine)
        {
            MagazineText.text = "Buy External Magazine" + "\n" + M.gunPrice.ToString();
            if (M.isBought) ChangeToTransparent(Magazine);
            else ChangeColorToGray(Magazine);
        }


        if (M.hasLaser) Limg.gameObject.SetActive(true);
        else Limg.gameObject.SetActive(false);
        if (M.laserPrice == 0 || M.hasLaser)
        {
            ChangeColorToGray(Laser);
            LaserText.text = null;
        }
        else if (M.laserPrice != 0 && !M.hasLaser)
        {
            LaserText.text = "Buy Laser" + "\n" + M.laserPrice.ToString();
            if (M.isBought) ChangeToTransparent(Laser);
            else ChangeColorToGray(Laser);
        }


        float f = M.FireRate / 60f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= f)
                FireRate.indicators[i].SetActive(true);
            else
                FireRate.indicators[i].SetActive(false);
        }
        f = M.initialDeviation / 2f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= (10 - f))
                Accuracy.indicators[i].SetActive(true);
            else
                Accuracy.indicators[i].SetActive(false);
        }
        f = M.initialReloadTime / 0.375f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= (10 - f))
                ReloadSpeed.indicators[i].SetActive(true);
            else
                ReloadSpeed.indicators[i].SetActive(false);
        }
        f = M.initialMaxMagazine / 4.5f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= f)
                MagazineCapacity.indicators[i].SetActive(true);
            else
                MagazineCapacity.indicators[i].SetActive(false);
        }
        f = M.initialRadius / 1f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= (10 - f))
                Secretiveness.indicators[i].SetActive(true);
            else
                Secretiveness.indicators[i].SetActive(false);
        }
    }


    private void ChangeColorToGray(Image img)
    {
        img.color = new Color(0.6f, 0.6f, 0.6f);
        Color currentColor = img.color;
        currentColor.a = 100f / 255f;
        img.color = currentColor;
    }
    private void ChangeToTransparent(Image img)
    {
        img.color = Color.white;
        Color currentColor = img.color;
        currentColor.a = 0f / 255f;
        img.color = currentColor;
    }
    public void BuyWeap()
    {
        GunInfoManager M = G.GunInfo[ix];
        if (!M.isBought && M.gunPrice <= MoneySystem.money)
        {
            M.isBought = true;
            MoneySystem.money -= M.gunPrice;
            LoadGunDates(ix);
            sound.OnBought();
        }
        else if (!M.isBought) sound.NoMoney();
        else sound.Click();

    }
    public void BuySilencer()
    {
        GunInfoManager M = G.GunInfo[ix];
        if (M.silencerPrice == 0) return;
        if (M.isBought && !M.hasSilencer && M.silencerPrice <= MoneySystem.money)
        {
            M.hasSilencer = true;
            MoneySystem.money -= M.gunPrice;
            LoadGunDates(ix);
            sound.OnBought();
        }
        else if (!M.hasSilencer) sound.NoMoney();
        else sound.Click();
    }
    public void BuyExtMagazine()
    {
        GunInfoManager M = G.GunInfo[ix];
        if (M.extMagazinePrice == 0) return;
        if (M.isBought && !M.hasExtMagazine && M.extMagazinePrice <= MoneySystem.money)
        {
            M.hasExtMagazine = true;
            MoneySystem.money -= M.extMagazinePrice;
            LoadGunDates(ix);
            sound.OnBought();
        }
        else if (!M.hasExtMagazine) sound.NoMoney();
        else sound.Click();
    }
    public void BuyLaser()
    {
        GunInfoManager M = G.GunInfo[ix];
        if(M.laserPrice == 0) return;
        if (M.isBought && !M.hasLaser && M.laserPrice <= MoneySystem.money)
        {
            M.hasLaser = true;
            M.Laser.SetActive(true);
            MoneySystem.money -= M.laserPrice;
            LoadGunDates(ix);
            sound.OnBought();
        }
        else if (!M.hasLaser) sound.NoMoney();
        else sound.Click();
    }
}
