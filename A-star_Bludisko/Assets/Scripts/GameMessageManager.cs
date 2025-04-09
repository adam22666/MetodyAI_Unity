using UnityEngine;
using TMPro;
using System.Collections;


public class GameMessageManager : MonoBehaviour
{
    private TextMeshProUGUI gameMessage; 

    void Start()
    {
        gameMessage = GetComponent<TextMeshProUGUI>();

        if (gameMessage == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on GameMessage object!");
            return;
        }

        gameMessage.text = "Zabi Goblina predtým, ako dosiahne cieľ!";
        gameMessage.enabled = true;

        StartCoroutine(HideMessageAfterTime(10f));
    }

    IEnumerator HideMessageAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameMessage.enabled = false;
    }
}
