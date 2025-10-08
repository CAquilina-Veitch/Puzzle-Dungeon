using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Runtime.Extensions
{
    public static class UIExtensions
    {
        public static bool IsPointerOverUI
        {
            get
            {
                // Check if there's an EventSystem
                if (EventSystem.current == null)
                    return false;
            
                // For mouse (desktop)
                if (Input.mousePresent)
                {
                    // Create a PointerEventData with the current mouse position
                    PointerEventData pointerData = new PointerEventData(EventSystem.current);
                    pointerData.position = Input.mousePosition;
            
                    // Raycast using the EventSystem
                    List<RaycastResult> results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, results);
            
                    // Check if any hit is on the UI layer (layer 5)
                    foreach (RaycastResult result in results)
                    {
                        if (result.gameObject != null && result.gameObject.layer == 5)
                            return true;
                    }
                }
        
                // For touch (iPad/mobile)
                if (Input.touchSupported && Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                
                        // Create a PointerEventData with the current touch position
                        PointerEventData pointerData = new PointerEventData(EventSystem.current);
                        pointerData.position = touch.position;
                
                        // Raycast using the EventSystem
                        List<RaycastResult> results = new List<RaycastResult>();
                        EventSystem.current.RaycastAll(pointerData, results);
                
                        // Check if any hit is on the UI layer (layer 5)
                        foreach (RaycastResult result in results)
                        {
                            if (result.gameObject != null && result.gameObject.layer == 5)
                                return true;
                        }
                    }
                }
        
                return false;
            }
        }
    }
}