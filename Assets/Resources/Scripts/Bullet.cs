using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    Renderer render;

    protected override void Awake()
    {
        base.Awake();
        speed = 40;
        damage = 10;
        render = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        if (GameSystem.instance.gameState == GameSystem.GameState.Shop)
        {
            render.sortingLayerName = "Default";
            render.sortingOrder = 3;
        }
        else
        {
            render.sortingLayerName = "Projectiles";
            render.sortingOrder = 0;
        }
    }
}
