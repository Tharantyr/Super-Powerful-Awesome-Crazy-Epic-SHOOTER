using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopRechargeShield : ShopItem
{
    protected override void Awake()
    {
        cost = 2;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (GameSystem.instance.shieldTime == 3 || GameSystem.instance.Orbs < cost)
            GetComponent<CanvasGroup>().alpha = 0.2f;
        else
            GetComponent<CanvasGroup>().alpha = 1;
    }

    public void Reset() // Used to reset the shop state upon exiting to main menu
    {
        cost = 2;
        transform.Find("RechargeShieldPrice").GetComponent<Text>().text = cost.ToString();
        transform.Find("RechargeShieldPic").Find("RechargeShieldDesc").GetComponent<Text>().text = "10s";
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (GameSystem.instance.Orbs >= cost && GameSystem.instance.shieldTime > 3)
        {
            GameSystem.instance.shieldTime -= 0.5f;
            GameSystem.instance.Orbs -= cost;
            transform.Find("RechargeShieldPic").Find("RechargeShieldDesc").GetComponent<Text>().text = GameSystem.instance.shieldTime.ToString() + "s";
            base.OnTriggerEnter2D(col);

            if (GameSystem.instance.shieldTime == 3)
                StartCoroutine(FadeOut());
        }
    }
}
