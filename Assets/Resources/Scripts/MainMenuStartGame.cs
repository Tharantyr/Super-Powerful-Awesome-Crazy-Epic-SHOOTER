﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuStartGame : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    Coroutine c;

    private void OnEnable()
    {
        GetComponent<Text>().color = Color.white;
        c = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (c == null)
            c = StartCoroutine(StartGame());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Text>().color = new Color(1, 0.91f, 0, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().color = Color.white;
    }

    private IEnumerator StartGame() // Transition to game when clicking button
    {
        GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.UISounds[0]); // Play sound effect
        GameSystem.instance.player.GetComponent<Renderer>().sortingOrder = 1;
        CanvasGroup c = transform.parent.GetComponent<CanvasGroup>();
        CanvasGroup uiCanvas = GameObject.Find("UI").GetComponent<CanvasGroup>();
        GameSystem.instance.InitializePlayer(); // Reset values for player upon new game

        while (c.alpha > 0)
        {
            c.alpha -= Time.deltaTime;
            uiCanvas.alpha += Time.deltaTime;
            yield return null;
        }

        GameSystem.instance.gameState = GameSystem.GameState.WaveStart;
        transform.parent.gameObject.SetActive(false);
    }
}
