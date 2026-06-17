using UnityEngine;
using TMPro;

public class InteractableView : MonoBehaviour
{
    public GameObject uiPanel;
    public Transform player;
    public Camera playerCam;
    public float interactDistance = 5f;
    public TextMeshProUGUI hintText;
    public AudioClip openSound;
    public AudioClip closeSound;

    static bool isViewing = false;
    static InteractableView current;

    PlayerMovement playerMovement;
    MouseLook mouseLook;
    AudioSource audioSource;

    void Start()
    {
        playerMovement = player.GetComponent<PlayerMovement>();
        mouseLook = playerCam.GetComponent<MouseLook>();
        audioSource = GetComponent<AudioSource>();
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

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.transform.name == transform.name || hit.transform.IsChildOf(transform))
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
        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);
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
        if (audioSource != null && closeSound != null)
            audioSource.PlayOneShot(closeSound);
    }
}