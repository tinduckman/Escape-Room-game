using UnityEngine;
using TMPro;

public class InteractableView : MonoBehaviour
{
    public GameObject uiPanel;
    public Transform player;
    public float interactDistance = 5f;
    public TextMeshProUGUI hintText;

    static bool isViewing = false;
    static InteractableView current;

    Transform playerCamera;
    PlayerMovement playerMovement;
    MouseLook mouseLook;

    void Start()
    {
        playerCamera = Camera.main.transform;
        playerMovement = player.GetComponent<PlayerMovement>();
        mouseLook = playerCamera.GetComponent<MouseLook>();
        uiPanel.SetActive(false);
        if (hintText != null) hintText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isViewing && current == this)
        {
            if (Input.GetKeyDown(KeyCode.E))
                StopViewing();
            return;
        }

        if (isViewing) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.transform == transform)
            {
                if (hintText != null) hintText.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hintText != null) hintText.gameObject.SetActive(false);
                    StartViewing();
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

    void StartViewing()
    {
        isViewing = true;
        current = this;
        uiPanel.SetActive(true);
        playerMovement.enabled = false;
        mouseLook.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void StopViewing()
    {
        isViewing = false;
        current = null;
        uiPanel.SetActive(false);
        playerMovement.enabled = true;
        mouseLook.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}