using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BodyArmor : MonoBehaviour
{
    public GameObject ArmorSprite;
    public int cost;
    public TMP_Text CostText;
    public bool hasArmor;
    public int Value = 1;
    public GameObject hasIcon;
    private SoundManager soundManager;

    public void BuyBodyArmor()
    {
        if (MoneySystem.money >= cost && !hasArmor)
        {
            MoneySystem.money -= cost;
            hasArmor = true;
            ArmorSprite.SetActive(true);
            Value = 1;
            hasIcon.SetActive(true);
            soundManager.OnBought();
        }
        else if (!hasArmor)
        {
            soundManager.NoMoney();
        }
        else soundManager.Click();
    }
    private void Start()
    {
        CostText.text = cost.ToString();
        soundManager = FindObjectOfType<SoundManager>();
    }
    public void Hit()
    {
        Value -= 1;
        if (Value <= 0) Broke();
    }
    private void Broke()
    {
        hasArmor = false;
        hasIcon.SetActive(false);
        ArmorSprite.SetActive(true);
    }
}
