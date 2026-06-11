using UnityEngine;
using static UnityEngine.Audio.ControlContext;

public class LatAlt : MonoBehaviour
{
    float correctLat, correctAlt;
    int correctWind;
    float playerLat, playerAlt;
    int playerWind;
    bool manualFound = false;
    const float NUDGE = 0.3f;
    public ConsoleMonitor console;
    public Manual manual;

    void Start()
    {
        correctLat = Random.Range(0f, 120f);
        correctAlt = Random.Range(0f, 120f);
        correctWind = Random.Range(30, 121);
        playerLat = 60f;
        playerAlt = 60f;
        playerWind = 75;

        FindManual();

        console.Log("GAME START");
        console.Log("P1: Adjust the levers");
    }

    void FindManual()
    {
        manualFound = true;
        manual.Reveal(correctLat, correctAlt, correctWind);
        Debug.Log("FindManual called! Lat: " + correctLat + " Alt: " + correctAlt + " Wind: " + correctWind);
    }

    public void AdjustLat(float val)
    {
        playerLat = val;
        playerAlt = Mathf.Clamp(playerAlt + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        playerWind = (int)Mathf.Clamp(playerWind + (Random.value > 0.5f ? 1 : -1), 30, 120);
        console.Log("Lat: " + playerLat.ToString("F1"));
        LogDiffs();
        CheckWin();
    }

    public void AdjustAlt(float val)
    {
        playerAlt = val;
        playerLat = Mathf.Clamp(playerLat + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        playerWind = (int)Mathf.Clamp(playerWind + (Random.value > 0.5f ? 1 : -1), 30, 120);
        console.Log("Alt: " + playerAlt.ToString("F1"));
        LogDiffs();
        CheckWin();
    }

    public void AdjustWind(int val)
    {
        playerWind = val;
        playerLat = Mathf.Clamp(playerLat + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        playerAlt = Mathf.Clamp(playerAlt + Random.Range(-NUDGE, NUDGE), 0f, 120f);
        console.Log("Wind: " + playerWind);
        LogDiffs();
        CheckWin();
    }

    void LogDiffs()
    {
        console.Log("Lat diff: " + Mathf.Abs(playerLat - correctLat).ToString("F1"));
        console.Log("Alt diff: " + Mathf.Abs(playerAlt - correctAlt).ToString("F1"));
        console.Log("Wind diff: " + Mathf.Abs(playerWind - correctWind));
    }

    void CheckWin()
    {
        if (Mathf.Abs(playerLat - correctLat) < 0.5f &&
            Mathf.Abs(playerAlt - correctAlt) < 0.5f &&
            Mathf.Abs(playerWind - correctWind) <= 1)
        {
            console.Log("MINIGAME COMPLETE");
        }
    }


}