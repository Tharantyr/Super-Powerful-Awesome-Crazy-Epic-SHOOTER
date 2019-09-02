﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopRechargeLives : ShopItem
{
    protected override void Awake()
    {
        cost = 20;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        transform.Find("RechargeLivesPic").GetChild(0).GetComponent<Text>().text = GameSystem.instance.Lives.ToString();

        if (GameSystem.instance.Lives == 3 || GameSystem.instance.Orbs < cost)
            GetComponent<CanvasGroup>().alpha = 0.2f;
        else
            GetComponent<CanvasGroup>().alpha = 1;
    }

    public void Reset() // Used to reset the shop state upon returning to main menu
    {
        cost = 20;
        transform.Find("RechargeLivesPrice").GetComponent<Text>().text = cost.ToString();
        transform.Find("RechargeLivesPic").Find("RechargeLivesDesc").GetComponent<Text>().text = GameSystem.instance.Lives.ToString();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        transform.Find("RechargeLivesPic").GetChild(0).GetComponent<Text>().text = GameSystem.instance.Lives.ToString();

        if (GameSystem.instance.Orbs >= cost && GameSystem.instance.Lives < 3)
        {
            GameSystem.instance.Lives++;
            GameSystem.instance.Orbs -= cost;
            base.OnTriggerEnter2D(col);

            if (GameSystem.instance.Lives == 3)
                StartCoroutine(FadeOut());
        }
    }
}
