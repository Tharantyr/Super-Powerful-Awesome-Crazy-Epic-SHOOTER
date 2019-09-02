using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUpgradeShield : ShopItem
{
    protected override void Awake()
    {
        cost = 5;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (GameSystem.instance.shieldCapacity == 100 || GameSystem.instance.Orbs < cost)
            GetComponent<CanvasGroup>().alpha = 0.2f;
        else
            GetComponent<CanvasGroup>().alpha = 1;
    }

    public void Reset() // Used to reset the shop state upon exiting to main menu
    {
        cost = 5;
        transform.Find("UpgradeShieldPrice").GetComponent<Text>().text = cost.ToString();
        transform.Find("UpgradeShieldPic").Find("UpgradeShieldDesc").GetComponent<Text>().text = "40";
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (GameSystem.instance.Orbs >= cost && GameSystem.instance.shieldCapacity < 100)
        {
            GameSystem.instance.shieldCapacity += 10;
            GameSystem.instance.Shield = GameSystem.instance.shieldCapacity;
            GameSystem.instance.Orbs -= cost;
            transform.Find("UpgradeShieldPic").Find("UpgradeShieldDesc").GetComponent<Text>().text = GameSystem.instance.shieldCapacity.ToString();
            base.OnTriggerEnter2D(col);

            if (GameSystem.instance.shieldCapacity == 100)
                StartCoroutine(FadeOut());
        }
    }
}
