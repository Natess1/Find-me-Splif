using TMPro;
using UnityEngine;

public class Buying : MonoBehaviour
{
    public int price;
    public TMP_Text textPrice;



    private int currentMoney;


    void Awake()
    {
        currentMoney = PlayerPrefs.GetInt("money");
        textPrice.text = price.ToString();
    }

    public void BuyBlade()
    {
        if (currentMoney >= price)
        {
            if (PlayerPrefs.GetInt("currentWeapon") != 2)
            {
                PlayerPrefs.SetInt("currentWeapon", 2);

            }
        }
    }

    public void BuySword()
    {
        if (currentMoney >= price)
        {
            if (PlayerPrefs.GetInt("currentWeapon") != 1)
            {
                PlayerPrefs.SetInt("currentWeapon", 1);
            }
        }
    }


}
