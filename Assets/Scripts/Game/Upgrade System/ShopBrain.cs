using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class ShopBrain : MonoBehaviour
{
    public static ShopBrain Instance;
    public bool active = false;

    public int charState = -1;
    // -1: Nobody
    // 0: Ansel
    // 1: Stella
    // 2: Zane

    public ShopScreenBase currentProcess = null;

    public ShopScreenBase callScreen;

    static bool initialized = false;

    public AudioSource audioSource;

    public GameObject StoreCanvas;

    public Canvas myCanvas;

    public int shopSeed;

    public GameObject Alert;
    public TextMeshProUGUI AlertText;

    bool inCutscene;

    private void Awake() {
        if (initialized) {
            Destroy(gameObject);
            return;
        }
        initialized = true;
        Instance = this;

        //GameEvents.OnCutsceneStateChanged += CutsceneStateChanged;
        GameEvents.OnShopStateChange += OnShopStateChange;
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        myCanvas.worldCamera = Camera.main;
    }

    public void OnShopStateChange(bool isOpen) {
        if(isOpen) {
            OpenShop();
        }
        else {
            CloseShop();
        }
    }

    public void OpenShop() {
        Debug.Log("help");
        if (active) {
            return;
        }
        Debug.Log("helppls");

        StoreCanvas.SetActive(true);
        MusicManager.Instance.StartCalm();

        if (!currentProcess) {
            currentProcess = callScreen;
        }
        active = true;
        currentProcess.gameObject.SetActive(true);
        currentProcess.Activate();
        shopSeed = Random.Range(0, int.MaxValue);
    }

    public void CloseShop() {
        if (!active)
            return;
        WaveEvents.ShopClose();

        StoreCanvas.SetActive(false);
        active = false;
    }
}
