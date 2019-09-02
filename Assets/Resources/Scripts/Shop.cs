using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    void OnEnable()
    {
        GameSystem.instance.gameState = GameSystem.GameState.Shop;
        GameSystem.instance.Health = 100;
        StartCoroutine(MoveToCenter(GameSystem.instance.player.transform.position)); // Fade in shop and move player to shop center
        transform.Find("OrbCount").GetComponent<Text>().text = "* " + GameSystem.instance.Orbs; // Update orb counter
    }

    public void Reset() // Used to reset the shop state upon exiting to main menu
    {
        transform.Find("RechargeLives").GetComponent<ShopRechargeLives>().Reset();
        transform.Find("RechargeShield").GetComponent<ShopRechargeShield>().Reset();
        transform.Find("RestockRockets").GetComponent<ShopRestockRockets>().Reset();
        transform.Find("UpgradeRockets").GetComponent<ShopUpgradeRockets>().Reset();
        transform.Find("UpgradeShield").GetComponent<ShopUpgradeShield>().Reset();
    }

    private IEnumerator MoveToCenter(Vector2 startPosition) // Transition to shop and move player to center and lock him in place
    {
        GameSystem.instance.player.GetComponent<Renderer>().sortingOrder = 4;
        Rigidbody2D playerBody = GameSystem.instance.player.GetComponent<Rigidbody2D>();
        Transform background = GameObject.Find("Background").transform;
        float startDistance = Vector2.Distance(playerBody.transform.position, new Vector2(0, -1.5f));
        float distance = startDistance;
        float shopAlpha = GetComponent<CanvasGroup>().alpha;

        while (distance > 0.2f)
        {
            playerBody.position = Vector2.MoveTowards(playerBody.position, new Vector2(0, -1.5f), Time.deltaTime * 10);
            background.position = Vector2.MoveTowards(background.position, Vector2.zero, Time.deltaTime * 10);
            distance = Vector2.Distance(playerBody.transform.position, new Vector2(0, -1.5f));

            if (shopAlpha < 1)
            {
                float val = distance / startDistance;
                shopAlpha = 1 - val;
                GetComponent<CanvasGroup>().alpha = shopAlpha;
                GameSystem.instance.UI.transform.Find("TopUI").GetComponent<CanvasGroup>().alpha = val;
            }

            yield return null;
        }

        shopAlpha = 1;
        GameSystem.instance.UI.transform.Find("TopUI").GetComponent<CanvasGroup>().alpha = 0;
        playerBody.velocity = Vector2.zero;

        for (int i = 2; i < 8; i++)
            transform.GetChild(i).GetComponent<Collider2D>().enabled = true;
    }

    public IEnumerator FadeShop() // Fade out shop upon exiting
    {
        GameSystem.instance.UI.transform.Find("WaveText").gameObject.SetActive(true);
        float shopAlpha = GetComponent<CanvasGroup>().alpha;

        while (shopAlpha > 0)
        {
            shopAlpha -= Time.deltaTime;
            GetComponent<CanvasGroup>().alpha = shopAlpha;
            GameSystem.instance.UI.transform.Find("TopUI").GetComponent<CanvasGroup>().alpha += Time.deltaTime;
            yield return null;
        }

        for (int i = 2; i < 8; i++)
        {
            transform.GetChild(i).GetComponent<Collider2D>().enabled = false; // Disable shop item bounding boxes so player doesn't collide with them when flying to center of shop next time
        }

        GameSystem.instance.gameState = GameSystem.GameState.WaveStart;
        GameSystem.instance.player.GetComponent<Renderer>().sortingOrder = 1;
        gameObject.SetActive(false);
    }
}
