using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{

    [SerializeField] Slider slider;
    [SerializeField] Image[] slots;
    [SerializeField] Sprite slotImage;
    [SerializeField] Sprite highlightSlotImage;

    public byte currentByte = 4;

    int inventoryIndex = 0;

    public void SetSlider(float maxDuration)
    {
        slider.gameObject.SetActive(true);
        slider.maxValue = maxDuration;
        slider.minValue = 0;
    }

    public void UpdateSlider(float current)
    {
        slider.value = current;
    }

    public void ResetSlider()
    {
        slider.value = 0;
        slider.gameObject.SetActive(false);
    }

    private void SetHighlightSlot()
    {
        UnhiglightSlot();
        slots[inventoryIndex].sprite = highlightSlotImage;
    }

    private void UnhiglightSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = slotImage;
        }
    }

    public void GetInventoryIndex(int i)
    {
        inventoryIndex += i;
        if (inventoryIndex >= slots.Length)
        {
            inventoryIndex = 0;
        }

        if (inventoryIndex < 0)
        {
            inventoryIndex = slots.Length - 1;
        }

        if(inventoryIndex == 0)
        {
            currentByte = 4;
        }
        else if(inventoryIndex == 1)
        {
            currentByte = 2;
        }
        else if (inventoryIndex == 2)
        {
            currentByte = 5;
        }
        else if (inventoryIndex == 3)
        {
            currentByte = 3;
        }

        SetHighlightSlot();
    }
}
