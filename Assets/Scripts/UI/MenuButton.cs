using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    bool isPressed = false;
    Toggle toggle; 
    RectTransform rect;

    public GameObject panel;
    void Start()
    {
        rect = GetComponent<RectTransform>();
        toggle = GetComponent<Toggle>();
        if(toggle.isOn)
        {
            toggle.Select();
            ToggleEvent();
        }
    }
  
    public void ToggleEvent()
    {
        if(toggle.isOn)
        {
            if(isPressed == false)
            {   
                rect.anchoredPosition += new Vector2(0f, 30f);
                panel.SetActive(true);
                buttonImage.sprite = selectedSprite;
            }

            isPressed = true;
        }
        else
        {
            if (isPressed == true)
            {
                rect.anchoredPosition += new Vector2(0f, -30f);
                panel.SetActive(false);
                buttonImage.sprite = normalSprite;
            }
            isPressed = false;
        }
    }

}
