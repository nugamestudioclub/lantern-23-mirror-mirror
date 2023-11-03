using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonScripts : MonoBehaviour
{
    public void StartGame()
    {
        GetComponent<Button>().enabled = false;
        GameObject text = this.transform.GetChild(0).gameObject;

        // note: main menu has index 0, gameplay has index 1, gameover has index 2
        StartCoroutine(StartGameClickedFlicker(text));
    }

    IEnumerator StartGameClickedFlicker(GameObject text)
    {
        RectTransform textTransform = text.GetComponent<RectTransform>();
        Vector2 originalPos = textTransform.anchoredPosition;
        Quaternion originalRotation = textTransform.rotation;

        for (int i = 0; i < 4; i += 1)
        {
            text.SetActive(false);
            yield return new WaitForSeconds(0.05f);
            text.SetActive(true);
            yield return new WaitForSeconds(0.1f);

            float xOffset = Random.Range(0, 1) > 0.5 ? Random.Range(10f, 25f) : Random.Range(-10f, -25f);
            float yOffset = Random.Range(0, 1) > 0.5 ? Random.Range(10f, 25f) : Random.Range(-10f, -25f);

            textTransform.anchoredPosition = originalPos + new Vector2(xOffset, yOffset);

            textTransform.rotation = originalRotation;
            textTransform.Rotate(Vector3.forward * Random.Range(-25, 25));
        }

        // note: main menu has index 0, gameplay has index 1, gameover has index 2
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
