using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using DG.Tweening;
using System.Reflection;

public class InfoToUI : MonoBehaviour
{

    public TMP_Text MagazineCount;
    public Image LaserImg;
    public Image SilencerImg;
    public Image ExtMagazineImg;
    public Image Icon;
    public TMP_Text GunName;
    public GameObject[] Guns;
    public GunInfoManager[] GunInfo;

    public GridObjects Accuracy;
    public GridObjects FireRate;
    public GridObjects MagazineCapacity;
    public GridObjects ReloadSpeed;
    public GridObjects Secretiveness;

    public List<GameObject> itemsToFade = new List<GameObject> ();

    private int ix;
    public void Load(int index)
    {
        GunInfoManager G = GunInfo[index];
        G.CalculateValues();
        StopAllCoroutines();
        MagazineCount.text = G.magazineCount.ToString();

        if (G.icon != Icon.sprite) ChangeIcon();
        else
        {
            if (G.laserPrice == 0) LaserImg.gameObject.SetActive(false);
            else
            {
                LaserImg.gameObject.SetActive(true);
                if (G.hasLaser && G.isEquipedLaser) LaserImg.color = Color.green;
                else if (G.hasLaser && !G.isEquipedLaser) LaserImg.color = new Color(0.6f, 0.6f, 0.6f);
                else LaserImg.color = Color.red;
            }

            if (G.silencerPrice == 0) SilencerImg.gameObject.SetActive(false);
            else
            {
                SilencerImg.gameObject.SetActive(true);
                if (G.hasSilencer && G.isEquipedSilencer) SilencerImg.color = Color.green;
                else if (G.hasSilencer && !G.isEquipedSilencer) SilencerImg.color = new Color(0.6f, 0.6f, 0.6f);
                else SilencerImg.color = Color.red;
            }

            if (G.extMagazinePrice == 0) ExtMagazineImg.gameObject.SetActive(false);
            else
            {
                ExtMagazineImg.gameObject.SetActive(true);
                if (G.hasExtMagazine && G.isEquipedExtMagazine) ExtMagazineImg.color = Color.green;
                else if (G.hasExtMagazine && !G.isEquipedExtMagazine) ExtMagazineImg.color = new Color(0.6f, 0.6f, 0.6f);
                else ExtMagazineImg.color = Color.red;
            }
        }

        GunName.text = G.GunName;
        float f = G.FireRate / 60f;
        for (int i = 0; i < 10; i++) 
        {
            if (i <= f)
                FireRate.indicators[i].SetActive(true);
            else
                FireRate.indicators[i].SetActive(false);
        }
        f = G.Accuracy / 2f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= (10 - f))
                Accuracy.indicators[i].SetActive(true);
            else
                Accuracy.indicators[i].SetActive(false);
        }
        f = G.Reload / 0.375f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= (10 - f))
                ReloadSpeed.indicators[i].SetActive(true);
            else
                ReloadSpeed.indicators[i].SetActive(false);
        }
        f = G.StoreCapacity / 4.5f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= f)
                MagazineCapacity.indicators[i].SetActive(true);
            else
                MagazineCapacity.indicators[i].SetActive(false);
        }
        f = G.Secretiveness / 1f;
        for (int i = 0; i < 10; i++)
        {
            if (i <= (10-f))
                Secretiveness.indicators[i].SetActive(true);
            else
                Secretiveness.indicators[i].SetActive(false);
        }
        /*Stats.text = "FireRate: " + G.FireRate.ToString() + "\n" +
             "StoreCapacity: " + G.StoreCapacity.ToString() + "\n" +  secrecy
             "Accuracy: " + G.Accuracy.ToString() + "\n" +
             "Reload: " + G.Reload.ToString();*/

    }

    private void ChangeIcon()
    {
        Transform tr = Icon.GetComponent<Transform>();
        CanvasGroup cg = tr.GetComponent<CanvasGroup>();
        cg.alpha = 1f;
        tr.transform.localScale = new Vector3(1f, 1f, 1f);
        tr.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.5f);
        cg.DOFade(0, 0.5f);
        Invoke("FadeIn", 0.5f);

        foreach (var item in itemsToFade)
        {
            item.GetComponent<CanvasGroup>().alpha = 1f;
        }

        foreach (var item in itemsToFade)
        {
            item.transform.transform.localScale = new Vector3(1f, 1f, 1f);
            item.transform.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.5f);
            item.GetComponent<CanvasGroup>().DOFade(0, 0.5f);
        }



    }
    private void FadeIn()
    {
        Transform tr = Icon.GetComponent<Transform>();
        CanvasGroup cg = tr.GetComponent<CanvasGroup>();
        Icon.sprite = GunInfo[ix].icon;
        tr.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        cg.DOFade(1, 0.5f);

        foreach (var item in itemsToFade)
        {
            item.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
            item.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        }

        GunInfoManager G = GunInfo[ix];
        if (G.laserPrice == 0) LaserImg.gameObject.SetActive(false);
        else
        {
            LaserImg.gameObject.SetActive(true);
            if (G.hasLaser && G.isEquipedLaser) LaserImg.color = Color.green;
            else if (G.hasLaser && !G.isEquipedLaser) LaserImg.color = new Color(0.6f, 0.6f, 0.6f);
            else LaserImg.color = Color.red;
        }

        if (G.silencerPrice == 0) SilencerImg.gameObject.SetActive(false);
        else
        {
            SilencerImg.gameObject.SetActive(true);
            if (G.hasSilencer && G.isEquipedSilencer) SilencerImg.color = Color.green;
            else if (G.hasSilencer && !G.isEquipedSilencer) SilencerImg.color = new Color(0.6f, 0.6f, 0.6f);
            else SilencerImg.color = Color.red;
        }

        if (G.extMagazinePrice == 0) ExtMagazineImg.gameObject.SetActive(false);
        else
        {
            ExtMagazineImg.gameObject.SetActive(true);
            if (G.hasExtMagazine && G.isEquipedExtMagazine) ExtMagazineImg.color = Color.green;
            else if (G.hasExtMagazine && !G.isEquipedExtMagazine) ExtMagazineImg.color = new Color(0.6f, 0.6f, 0.6f);
            else ExtMagazineImg.color = Color.red;
        }
    }
    private void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            Image _icon = Guns[i].GetComponent<Image>();
            _icon.sprite = GunInfo[i].icon;
        }
    }
    private void OnEnable()
    {
        foreach (GunInfoManager Gun in GunInfo)
        {
            int index = Gun.index;
            GameObject GunButton = Guns[index].gameObject;
            if (Gun.isBought) GunButton.SetActive(true); else GunButton.SetActive(false);

        }
    }
    public void FixedUpdate()
    {
        foreach (GunInfoManager script in GunInfo)
        {
            if (script.isActiveAndEnabled)
            {
                Load(script.index);
                ix = script.index;
                return;
            }
        }
    }
    public void ChangeLaser()
    {
        GunInfo[ix].ToggleLaser();

    }
    public void ChangeSilencer()
    {
        GunInfo[ix].ToggleSilencer();
    }
    public void ChangeExtMagazine()
    {
        GunInfo[ix].ToggleExtMagazine();
    }
}
