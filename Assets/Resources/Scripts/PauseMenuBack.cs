using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuBack : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
            c = StartCoroutine(Back());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Text>().color = new Color(1, 0.91f, 0, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().color = Color.white;
    }

    private IEnumerator Back() // Transition to main menu when pressing button
    {
        GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.UISounds[0]); // Play sound effect
        foreach (GameObject o in GameSystem.instance.enemyList) // Destroy all enemies in currently active enemy list
        {
            if (o.activeInHierarchy)
            o.GetComponent<Enemy>().WaveDeath();
        }
        GameSystem.instance.enemyList = new List<GameObject>();

        GameSystem.instance.shop.GetComponent<Shop>().Reset();
        GameSystem.instance.shop.SetActive(false);
        GameSystem.instance.player.SetActive(false);
        GameSystem.instance.enemySpawner.SetActive(false);

        CanvasGroup mainMenuCanvas = ObjectPool.instance.GetPooledObject("MainMenu").GetComponent<CanvasGroup>();

        while (mainMenuCanvas.alpha < 1)
        {
            mainMenuCanvas.alpha += Time.deltaTime;
            yield return null;
        }

        GameSystem.instance.gameState = GameSystem.GameState.MainMenu;
        transform.parent.gameObject.SetActive(false);
    }
}
