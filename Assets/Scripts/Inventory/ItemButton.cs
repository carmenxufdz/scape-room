using TMPro;
using UnityEngine;

namespace UnityEngine.XR.ARFoundation.Samples
{
    public class ItemButton : MonoBehaviour
    {
        public TMP_Text itemNameText;
        public UI.Image itemIconImage;
        public GameObject highlight;

        private Item itemData;

        public void SetData(Item data)
        {
            itemData = data;
            itemNameText.text = data.itemName;
            itemIconImage.sprite = data.icon;

            GetComponent<UI.Button>().onClick.AddListener(OnClick);
        }

        void OnClick()
        {

            if (PlayerManager.Instance.GetHeldItem() == itemData)
            {
                PlayerManager.Instance.ClearHeldItem();
                InventoryManager.Instance.UpdateSelection(null); // quita borde
                Debug.Log("Objeto en mano deseleccionado: " + itemData.itemName);
            }
            else
            {
                PlayerManager.Instance.SetHeldItem(itemData);
                InventoryManager.Instance.UpdateSelection(this); // quita borde
                Debug.Log("Objeto en mano seleccionado: " + itemData.itemName);

            }
        }

        public void SetSelected(bool selected)
        {
            highlight.SetActive(selected);
        }
    }
}
