using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class PlayerManager : MonoBehaviour
    {
        private Item heldItem;
        public static PlayerManager Instance;

        public void Awake()
        {
            Instance = this;
        }

        public void SetHeldItem(Item item)
        {
            heldItem = item;
            Debug.Log("Jugador tiene en mano: " + item.itemName);
        }

        public Item GetHeldItem() 
        {
            return heldItem; 
        }

        public void ClearHeldItem()
        {
            heldItem = null;
        }

        public bool IsHoldingItem(string itemName)
        {
            return heldItem != null && heldItem.itemName == itemName; 
        }

        public bool IsHoldingItem()
        {
            return heldItem != null;
        }
    
    }
}
