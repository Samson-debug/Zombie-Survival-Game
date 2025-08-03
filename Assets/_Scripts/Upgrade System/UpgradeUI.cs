using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeSystem
{
    public class UpgradeUI : MonoBehaviour
    {
        [Header("XP Setting")]
        public Slider xpSlider;
        
        [Header("Options Settings")]
        public GameObject upgradeOptionPanel;
        public TextMeshProUGUI leftUpgradeText;
        public TextMeshProUGUI rightUpgradeText;
        public Image leftUpgradeIcon;
        public Image rightUpgradeIcon;

        private System.Action<BaseUpgrade> onUpgradeSelected;
        private List<BaseUpgrade> currentOptions;

        private void Start()
        {
            upgradeOptionPanel.SetActive(false);
        }

        private void OnEnable()
        {
            ExperienceManager.OnXPChanged += UpdateSlider;
        }

        private void OnDisable()
        {
            ExperienceManager.OnXPChanged -= UpdateSlider;
        }

        private void UpdateSlider(float upgradePoints, float xpRequiredForNextUpgrade)
        {
            xpSlider.value = upgradePoints / xpRequiredForNextUpgrade;
        }

        public void ShowUpgradeOptions(List<BaseUpgrade> options, System.Action<BaseUpgrade> onSelected)
        {
            currentOptions = options;
            onUpgradeSelected = onSelected;
            
            SetupUpgradeButton(leftUpgradeText, leftUpgradeIcon, options[0]); 
            SetupUpgradeButton(rightUpgradeText, rightUpgradeIcon, options[1]);

            upgradeOptionPanel.SetActive(true);
        }
        
        private void SetupUpgradeButton(TextMeshProUGUI text, Image icon, BaseUpgrade upgrade)
        {
            if (upgrade != null){
                text.text = upgrade.description;
                //icon.sprite = upgrade.icon;
                //icon.color = upgrade.rarityColor;
            }
        }

        public void SelectUpgrade(int index)
        {
            if (index < currentOptions.Count){
                //selection effect
                onUpgradeSelected?.Invoke(currentOptions[index]);
            }
        }

        public void HideUpgradeOptions()
        {
            upgradeOptionPanel.SetActive(false);
        }
    }
}