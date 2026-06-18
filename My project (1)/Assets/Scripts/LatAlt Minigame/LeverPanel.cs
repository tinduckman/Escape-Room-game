using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeverPanel : MonoBehaviour
{
    public GameObject uiPanel;
    public Camera playerCam;
    public float interactDistance = 5f;
    public TextMeshProUGUI hintText;

    public Slider latSlider;
    public Slider altSlider;
    public Slider windSlider;
    public LatAlt latAlt;

    bool isOpen = false; // FIXED: was static, now per-instance
    MouseLook1 mouseLook;

    void Start()
    {
        mouseLook = playerCam.GetComponent<MouseLook1>();
        uiPanel.SetActive(false);
        if (hintText != null) hintText.gameObject.SetActive(false);

        latSlider.minValue = 0f;
        latSlider.maxValue = 120f;
        latSlider.value = 60f;

        altSlider.minValue = 0f;
        altSlider.maxValue = 120f;
        altSlider.value = 60f;

        windSlider.minValue = 30f;
        windSlider.maxValue = 120f;
        windSlider.wholeNumbers = true;
        windSlider.value = 75f;

        latSlider.onValueChanged.AddListener(OnLatChanged);
        altSlider.onValueChanged.AddListener(OnAltChanged);
        windSlider.onValueChanged.AddListener(OnWindChanged);
    }

    void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.E)) ClosePanel();
            return;
        }

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.transform == transform)
            {
                if (hintText != null) hintText.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hintText != null) hintText.gameObject.SetActive(false);
                    OpenPanel();
                }
            }
            else
            {
                if (hintText != null) hintText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (hintText != null) hintText.gameObject.SetActive(false);
        }
    }

    void OnLatChanged(float val) => latAlt.AdjustLat(val);
    void OnAltChanged(float val) => latAlt.AdjustAlt(val);
    void OnWindChanged(float val) => latAlt.AdjustWind((int)val);

    void OpenPanel()
    {
        isOpen = true;
        uiPanel.SetActive(true);
        mouseLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ClosePanel()
    {
        isOpen = false;
        uiPanel.SetActive(false);
        mouseLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}