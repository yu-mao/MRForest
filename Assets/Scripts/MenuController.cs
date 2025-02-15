using System;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Define an event to notify listeners when the menu is disabled.
    public event Action MenuDisabled;

    private void OnDisable()
    {
        // Fire the event when this GameObject is disabled.
        MenuDisabled?.Invoke();
    }
}