using UnityEngine;
using TMPro;

public class Manual : MonoBehaviour
{
    public TextMeshProUGUI latText;
    public TextMeshProUGUI altText;
    public TextMeshProUGUI windText;

    public void Reveal(float lat, float alt, int wind)
    {
        Debug.Log("Reveal called! Lat: " + lat + " Alt: " + alt + " Wind: " + wind);
        latText.text = "Lat: " + lat.ToString("F1");
        altText.text = "Alt: " + alt.ToString("F1");
        windText.text = "Wind: " + wind;
        Debug.Log("LatText is now: " + latText.text);
    }
}