using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace Scripts.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private health healthComponent;
        [SerializeField] private HealthHeartUI heartPrefab;
        private readonly List<HealthHeartUI> hearts = new();

        private void Start()
        {
            SetMaxHealth(healthComponent.currentHealth.Get);
            healthComponent.currentHealth.Subscribe(UpdateHealth).AddTo(this);
        }

        private void SetMaxHealth(int maxHealth)
        {
            if (maxHealth > hearts.Count)
                for (var i = hearts.Count; i < maxHealth; i++)
                    hearts.Add(Instantiate(heartPrefab, transform));
            else if (maxHealth < hearts.Count)
                while (maxHealth < hearts.Count)
                {
                    var index = hearts.Count - 1;
                    Destroy(hearts[index].gameObject);
                    hearts.RemoveAt(index);
                }
        }

        private void UpdateHealth(int newHealth)
        {
            for (var i = 0; i < hearts.Count; i++)
            {
                hearts[i].SetState(i < newHealth ? HealthHeartUI.HeartState.Full : HealthHeartUI.HeartState.Empty);
            }
        }
    }
}