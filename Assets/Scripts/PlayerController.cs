using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float reverseSpeed = -5f;
    public float turningSpeed = 5f;
    public float verticalSpeed = 5f;

    public float maxForwardVelocity = 10f;
    public float maxReverseVelocity = -5f;
    public float maxTurningVelocity = 1.5f;
    public float maxVerticalVelocity = 5f;

    public float minPitch = 1f;
    public float maxPitch = 2.5f;
    public float maxPitchVelocity = 70f;

    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI inventoryText;
    public Material completedKeyBoxMaterial;

    public GameManager gm;

    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public AudioSource audioSource;

    int redKeysQuantity = 0;
    int greenKeysQuantity = 0;
    int blueKeysQuantity = 0;

    [HideInInspector]
    public bool interactionReady = false;
    [HideInInspector]
    public bool playingMinigame = false;
    string minigameName;
    KeyBoxController curKeyBox;
    CageController curCage;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxTurningVelocity;
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(interactionReady && Input.GetKeyDown(KeyCode.E))
        {
            if (curKeyBox != null)
            {
                interactionText.gameObject.SetActive(false);
                playingMinigame = true;
                rb.Sleep();
                interactionReady = false;
                switch(minigameName)
				{
                    case "Reaction":
                        gm.StartReactionMinigame(curKeyBox.difficulty);
                        break;
                    case "Memory":
                        gm.StartMemoryMinigame(curKeyBox.difficulty);
                        break;
                    default:
                        Debug.Log("Unknown minigame to start!");
                        playingMinigame = false;
                        break;
                }
            }
            else if(curCage != null)
			{
                interactionText.gameObject.SetActive(false);
                rb.Sleep();
                interactionReady = false;
                redKeysQuantity -= curCage.redKeys;
                greenKeysQuantity -= curCage.greenKeys;
                blueKeysQuantity -= curCage.blueKeys;
                UpdateInventoryText();
                gm.ShowCreaturesMenu();
                StartCoroutine(animateCreatureRelease());
            }
        }
    }

    void FixedUpdate()
    {
        if (!playingMinigame)
        {
            if (Input.GetAxisRaw("Vertical") > 0 && transform.InverseTransformDirection(rb.velocity).z < maxForwardVelocity)
                rb.AddForce(transform.forward * forwardSpeed);
            else if (Input.GetAxisRaw("Vertical") < 0 && transform.InverseTransformDirection(rb.velocity).z > maxReverseVelocity)
                rb.AddForce(transform.forward * reverseSpeed);

            if (Input.GetAxisRaw("Horizontal") > 0)
                rb.AddRelativeTorque(new Vector3(0, turningSpeed, 0));
            else if (Input.GetAxisRaw("Horizontal") < 0)
                rb.AddRelativeTorque(new Vector3(0, -turningSpeed, 0));

            if (Input.GetAxisRaw("Submarine Vertical") > 0 && transform.InverseTransformDirection(rb.velocity).y < maxVerticalVelocity)
                rb.AddForce(transform.up * verticalSpeed);
            if (Input.GetAxisRaw("Submarine Vertical") < 0 && transform.InverseTransformDirection(rb.velocity).y > -maxVerticalVelocity)
                rb.AddForce(-transform.up * verticalSpeed);
        }

        audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.Abs(rb.velocity.sqrMagnitude) / maxPitchVelocity);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Key Box")
        {
            curKeyBox = other.gameObject.GetComponent<KeyBoxController>();
            if (curKeyBox.completed)
                return;

            string keyColorTag;
            string difficulty;

            switch(curKeyBox.minigame)
            {
                case 0:
                    minigameName = "Reaction";
                    break;
                case 1:
                    minigameName = "Memory";
                    break;
                default:
                    minigameName = "UNKNOWN_NAME";
                    Debug.Log("Unknown minigame");
                    break;
            }

            switch(curKeyBox.keyColor)
            {
                case "red":
                    keyColorTag = "#FF0000";
                    break;
                case "green":
                    keyColorTag = "#00FF00";
                    break;
                case "blue":
                    keyColorTag = "#0000FF";
                    break;
                default:
                    keyColorTag = "#FFFFFF";
                    Debug.Log("Unknown keyColor");
                    break;
            }

            switch(curKeyBox.difficulty)
			{
                case 0:
                    difficulty = "Easy";
                    break;
                case 1:
                    difficulty = "Medium";
                    break;
                case 2:
                    difficulty = "Hard";
                    break;
                default:
                    difficulty = "UNKNOWN_DIFFICULTY";
                    Debug.Log("Unknown difficulty");
                    break;
            }

            interactionText.text = "Press 'E' to play " + minigameName + " (" + difficulty + ")\nminigame and earn " +
                "<sprite=0 color=" + keyColorTag + ">";
            interactionText.gameObject.SetActive(true);
            interactionReady = true;
        }
        else if(other.gameObject.tag == "Cage")
        {
            curCage = other.gameObject.GetComponent<CageController>();

            bool enoughKeys = redKeysQuantity >= curCage.redKeys && greenKeysQuantity >= curCage.greenKeys &&
                blueKeysQuantity >= curCage.blueKeys;

            interactionText.text = "Requirements to open:\n" + (curCage.redKeys > 0 ? "<sprite=0 color=#FF0000>x" +
                curCage.redKeys : string.Empty) + " " + (curCage.greenKeys > 0 ? "<sprite=0 color=#00FF00>x" +
                curCage.greenKeys : string.Empty) + " " + (curCage.blueKeys > 0 ? "<sprite=0 color=#0000FF>x" +
                curCage.blueKeys : string.Empty) + "\n" + (enoughKeys ? "Press 'E' to open cage" : "Not enough keys");
            interactionText.gameObject.SetActive(true);
            interactionReady = enoughKeys;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Key Box" || other.gameObject.tag == "Cage")
        {
            curKeyBox = null;
            curCage = null;
            interactionText.gameObject.SetActive(false);
            interactionReady = false;
        }
    }

    public void MinigameCompleted()
    {
        playingMinigame = false;

        switch(curKeyBox.keyColor)
        {
            case "red":
                redKeysQuantity++;
                break;
            case "green":
                greenKeysQuantity++;
                break;
            case "blue":
                blueKeysQuantity++;
                break;
            default:
                break;
        }

        audioSource.UnPause();
        gm.musicSource.clip = gm.flyMusicClip;
        gm.musicSource.Play();
        gm.effectsSource.PlayOneShot(gm.keyAcquiredClip, 0.8f);
        curKeyBox.completed = true;
        curKeyBox.transform.GetComponentInChildren<MeshRenderer>().material = completedKeyBoxMaterial;
        UpdateInventoryText();
    }

    public void UpdateInventoryText()
    {
        inventoryText.text = "<sprite=0 color=#FF0000>x" + redKeysQuantity + "  <sprite=0 color=#00FF00>x" +
            greenKeysQuantity + "  <sprite=0 color=#0000FF>x" + blueKeysQuantity;
    }

    IEnumerator animateCreatureRelease()
	{
        gm.releasingAnimationGoing = true;
        CreatureButton button = curCage.creatureButton;
        Destroy(curCage.gameObject);
        button.transform.Find("Lock icon").GetComponent<Animator>().Play("Lock fade", -1, 0);
        gm.effectsSource.PlayOneShot(gm.unlockClip, 0.8f);
        yield return new WaitForSecondsRealtime(2);
        Image image = button.GetComponent<Image>();
        image.sprite = button.image;
        image.color = gm.whiteColor;
        button.GetComponent<Button>().interactable = true;
        gm.releasingAnimationGoing = false;
    }
}
