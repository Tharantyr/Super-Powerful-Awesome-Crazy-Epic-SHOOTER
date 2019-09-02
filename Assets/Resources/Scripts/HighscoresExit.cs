using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HighscoresExit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    CanvasGroup mainMenuCanvas;

    public void Awake()
    {
        mainMenuCanvas = ObjectPool.instance.pooledObjects.Find(o => o.tag == "MainMenu").GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        GetComponent<Text>().color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mainMenuCanvas.alpha == 0)
            StartCoroutine(BackToMainMenu());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Text>().color = new Color(1, 0.91f, 0, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().color = Color.white;
    }

    private IEnumerator BackToMainMenu() // Transition back to main menu when pressing this button
    {
        mainMenuCanvas.gameObject.SetActive(true);
        GameSystem.instance.soundManager.PlayOneShot(GameSystem.instance.UISounds[0]); // Play sound effect
        CanvasGroup canvas = transform.parent.parent.GetComponent<CanvasGroup>();

        while (canvas.alpha > 0)
        {
            canvas.alpha -= Time.deltaTime;
            mainMenuCanvas.alpha += Time.deltaTime;
            yield return null;
        }

        GameSystem.instance.gameState = GameSystem.GameState.MainMenu;
        transform.parent.parent.gameObject.SetActive(false);
    }
}
