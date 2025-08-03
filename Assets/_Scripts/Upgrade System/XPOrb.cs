using System;
using UnityEngine;

namespace UpgradeSystem
{
    
    [RequireComponent(typeof(Collider))]
    public class XPOrb : MonoBehaviour
    {
        [Header("Orb Settings")]
        public float xpValue = 10f;

        private void Start()
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null) collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
                CollectOrb();
        }

        private void CollectOrb()
        {
            var expManager = FindObjectOfType<ExperienceManager>();
            if (expManager != null){
                expManager.AddXP(xpValue);
            }

            Destroy(gameObject);
        }
    }
}