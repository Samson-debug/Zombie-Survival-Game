/*using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    //Singleton
    public static UpgradeManager Instance{ get; private set; }
    
    [Header("Option")]
    public Transform optionPanel;
    public RectTransform leftOptionTransform;
    public RectTransform rightOptionTransform;
    public Ease optionShrinkEase;
    
    [Range(0f, 1f)] public float timeScale = .2f;
    public float requiredUpgradePoints = 100;
    public float upgradePointsIncreaseRate = 1;
    public Slider slider;
    private float upgradePoints;
    private bool canUpgrade;

    private FirstPersonController playerMovementController;
    private HandBlaster playerNewHandBlaster;
    
    public bool Paused{ get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        
        var player = GameObject.FindGameObjectWithTag("Player");
        playerMovementController = player.GetComponent<FirstPersonController>();
        playerNewHandBlaster = player.GetComponent<HandBlaster>();
        
        optionPanel.gameObject.SetActive(false);
        leftOptionTransform.gameObject.SetActive(false);
        rightOptionTransform.gameObject.SetActive(false);
    }

    private void Start()
    {
        upgradePoints = 0;
        canUpgrade = false;
        Paused = false;
        
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (canUpgrade){
            if (Input.GetMouseButtonDown(0)) Choose(true);
            else if (Input.GetMouseButtonDown(1)) Choose(false);
            return;
        }
        
        upgradePoints += upgradePointsIncreaseRate * Time.deltaTime;
        upgradePoints = Mathf.Clamp(upgradePoints, 0f, requiredUpgradePoints);
        
        UpdateSlider();

        if (upgradePoints >= requiredUpgradePoints && !canUpgrade){
            StartUpgrade();
        }
        
        print($"upgrade points : {upgradePoints:F2}");
    }
    
    private void StartUpgrade()
    {
        upgradePoints = 0;
        
        Time.timeScale = timeScale;
        canUpgrade = true;
        Paused = true;
        
        optionPanel.gameObject.SetActive(true);
        leftOptionTransform.gameObject.SetActive(true);
        rightOptionTransform.gameObject.SetActive(true);
    }

    public void Choose(bool left)
    {
        canUpgrade = false;
        
        if (left){
            playerNewHandBlaster.UpgradeFireRate(15);
            leftOptionTransform.DOScale(0.7f, 0.7f)
                .SetEase(optionShrinkEase)
                .SetUpdate(true)
                .OnComplete(UnpauseGame);
        }
        else{
            playerMovementController.IncreaseSpeed(15);
            rightOptionTransform.DOScale(0.7f, 0.7f)
                .SetEase(optionShrinkEase)
                .SetUpdate(true)
                .OnComplete(UnpauseGame);
            
        }
        
        void UnpauseGame()
        {
            leftOptionTransform.localScale = Vector3.one;
            rightOptionTransform.localScale = Vector3.one;
            optionPanel.gameObject.SetActive(false);
            leftOptionTransform.gameObject.SetActive(false);
            rightOptionTransform.gameObject.SetActive(false);
            
            Time.timeScale = 1f;
            Paused = false;
        }
    }

    private void UpdateSlider()
    {
        slider.value = upgradePoints / requiredUpgradePoints;
    }

    public void GotOrb(float _upgradePoints)
    {
        upgradePoints += _upgradePoints;
    }
}*/