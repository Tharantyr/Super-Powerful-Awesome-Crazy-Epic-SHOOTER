using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuContinue : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameSystem.GameState previousState;

    private void OnEnable()
    {
        GetComponent<Text>().color = Color.white;
        previousState = GameSystem.instance.gameState; // Store previous state so we can get back to it when pressing continue
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.UISounds[0]); // Play sound effect
        GameSystem.instance.gameState = previousState;
        transform.parent.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Text>().color = new Color(1, 0.91f, 0, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().color = Color.white;
    }
}
