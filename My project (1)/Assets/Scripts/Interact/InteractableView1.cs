using UnityEngine;
using TMPro;

public class InteractableView1 : MonoBehaviour
{
    public GameObject uiPanel;
    public Transform player;
    public Camera playerCam;
    public float interactDistance = 5f;
    public TextMeshProUGUI hintText;
    public AudioClip openSound;
    public AudioClip closeSound;

    // ❌ ONLY FIX: remove shared static state
    bool isViewing = false;

    PlayerMovement playerMovement;
    MouseLook1 MouseLook1;
    AudioSource audioSource;

    void Start()
    {
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();

        if (playerCam != null)
            MouseLook1 = playerCam.GetComponent<MouseLook1>();

        audioSource = GetComponent<AudioSource>();

        if (uiPanel != null)
            uiPanel.SetActive(false);

        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerCam == null) return;

        if (isViewing)
        {
            if (Input.GetKeyDown(KeyCode.E))
                StopViewing();
            return;
        }

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.transform.name == transform.name || hit.transform.IsChildOf(transform))
            {
                if (hintText != null)
                    hintText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hintText != null)
                        hintText.gameObject.SetActive(false);

                    StartViewing();
                }
            }
            else
            {
                if (hintText != null)
                    hintText.gameObject.SetActive(false);
            }
        }
        else
        {
            if (hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }

    void StartViewing()
    {
        isViewing = true;

        if (uiPanel != null)
            uiPanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (MouseLook1 != null)
            MouseLook1.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (audioSource != null && openSound != null)
            audioSource.PlayOneShot(openSound);
    }

    void StopViewing()
    {
        isViewing = false;

        if (uiPanel != null)
            uiPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (MouseLook1 != null)
            MouseLook1.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (audioSource != null && closeSound != null)
            audioSource.PlayOneShot(closeSound);
    }
}