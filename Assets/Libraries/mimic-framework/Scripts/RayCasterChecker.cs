using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Mimic {

    [Serializable]
    public class RayCasterChecker {

        [SerializeField]
        private List<BaseRaycaster> rayCasters;
        public List<BaseRaycaster> RayCasters {
            set => rayCasters = value;
        }

        private PointerEventData pointerEventData;

        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        public RayCasterChecker(EventSystem eventSystem) : this((List<BaseRaycaster>)null, eventSystem) { 
        }

        public RayCasterChecker(List<BaseRaycaster> rayCasters, EventSystem eventSystem) {
            this.rayCasters = rayCasters;
            pointerEventData = new PointerEventData(eventSystem);
        }

        public RayCasterChecker(BaseRaycaster rayCaster, EventSystem eventSystem) : this(new List<BaseRaycaster>(){ rayCaster }, eventSystem) {
        }

        /// <summary>
        /// Checks whether there is a Ray Caster on the mouse position
        /// </summary>
        /// <returns>True when there is a ray caster (possible UI element on the mouse position).</returns>
        public bool CheckIfThereIsRayCaster() {
            if(rayCasters == null) {
                return false;
            }

            // Set the Pointer Event Position to that of the mouse position
            pointerEventData.position = Input.mousePosition;
            
            int rayCasterCount = rayCasters.Count;
            for (int i = 0; i < rayCasterCount; i++) {                
                // Raycast using the Graphics Raycaster and mouse click position
                rayCasters[i].Raycast(pointerEventData, raycastResults);
                if(raycastResults.Count > 0) {
                    raycastResults.Clear();
                    return true;
                }
            }

            return false;
        }

    }

}