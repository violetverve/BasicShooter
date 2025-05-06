using UnityEngine;
using System;

namespace BasicShooter.Player
{
    /// <summary>
    /// Handles damage reception and notifies listeners when health changes.
    /// </summary>
    public class Health : MonoBehaviour
    {
        public event Action OnHealthChanged;

        public void TakeDamage()
        {
            OnHealthChanged?.Invoke();
        }
    }
}