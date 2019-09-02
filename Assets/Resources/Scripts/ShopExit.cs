using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopExit : ShopItem
{
    Coroutine c;

    protected override void Awake() // Empty method so ExitShop doesn't go invisible like other shop items
    {
    }

    protected override void OnEnable()
    {
        GetComponent<Text>().color = new Color(0.76f, 0.76f, 0.76f, 1);
        itemPic = transform.GetChild(0);
        c = null;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.UISounds[0]); // Play sound effect
        GameObject obj = ObjectPool.instance.GetPooledObject("NormalEnemyExplosion"); // Spawn explosion
        obj.transform.localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.GetChild(0).localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.GetChild(1).localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.GetChild(2).localScale = new Vector3(0.05f, 0.05f, 1);
        obj.transform.position = GetComponent<Collider2D>().ClosestPoint(col.transform.position);
        obj.transform.rotation = transform.rotation;

        col.gameObject.SetActive(false);

        if (c == null)
        {
            StartCoroutine(FlyToPlayer(itemPic.position));
            c = StartCoroutine(GameObject.FindGameObjectWithTag("Shop").GetComponent<Shop>().FadeShop());
        }
    }
}
