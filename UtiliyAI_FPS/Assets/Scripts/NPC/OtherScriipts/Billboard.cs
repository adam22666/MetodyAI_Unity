using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TL.Core;

namespace TL.UI
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI bestActionText;
        [SerializeField] private TextMeshProUGUI actionScoresText;
        private Transform mainCameraTransform;

        void Start()
        {
            mainCameraTransform = Camera.main.transform;
        }

        void LateUpdate()
        {
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
        }
        
        public void UpdateActionScoresText(string scores)
        {
            if(actionScoresText != null)
                actionScoresText.text = scores;
        }

        public void UpdateStatsText(int health, int ammo, int grenades)
        {
            statsText.text = $"Health: {health}\nAmmo: {ammo}\nGrenades: {grenades}";
        }

        public void UpdateBestActionText(string bestAction)
        {
            bestActionText.text = bestAction;
        }
    }
}
