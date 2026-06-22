using UnityEngine;

public class LatAlt : MonoBehaviour
{
    float correctLat, correctAlt;
    int correctWind;
    float playerLat, playerAlt;
    int playerWind;
    const float NUDGE = 0.3f;

    public ConsoleMonitor console;
    public Manual manual;

    void Start()
    {
        correctLat = 31.8f;
        correctAlt = 70.4f;
        correctWind = 97;
        playerLat = 60f;
        playerAlt = 60f;
        playerWind = 75;

        manual.Reveal(correctLat, correctAlt, correctWind);

        console.Log("GAME START");
        console.Log("P1: Adjust the levers");
    }

    public void AdjustLat(float val)
    {
        playerLat = val;
        playerAlt = Mathf.Clamp(playerAlt + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        playerWind = (int)Mathf.Clamp(playerWind + (Random.value > 0.5f ? 1 : -1), 30, 120);
        LogValues();
        CheckWin();
    }

    public void AdjustAlt(float val)
    {
        playerAlt = val;
        playerLat = Mathf.Clamp(playerLat + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        playerWind = (int)Mathf.Clamp(playerWind + (Random.value > 0.5f ? 1 : -1), 30, 120);
        LogValues();
        CheckWin();
    }

    public void AdjustWind(int val)
    {
        playerWind = val;
        playerLat = Mathf.Clamp(playerLat + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        playerAlt = Mathf.Clamp(playerAlt + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        LogValues();
        CheckWin();
    }

    void LogValues()
    {
        console.Log("Lat: " + playerLat.ToString("F1"));
        console.Log("Alt: " + playerAlt.ToString("F1"));
        console.Log("Wind: " + playerWind);
    }

    void CheckWin()
    {
        if (Mathf.Abs(playerLat - correctLat) < 0.9f &&
            Mathf.Abs(playerAlt - correctAlt) < 0.9f &&
            Mathf.Abs(playerWind - correctWind) <= 1)
        {
            console.Log("MINIGAME COMPLETE");
        }
    }
}