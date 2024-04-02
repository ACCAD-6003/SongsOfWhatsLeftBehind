using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public abstract class UIButton : MonoBehaviour, IButton, IPointerEnterHandler
{
    public event Action<IButton> OnSelect;
    public event Action<IButton> OnClick;
    protected bool isSelected;
    private Button button;
    
    public void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Click);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected) OnSelect?.Invoke(this);
    }

    public void Click()
    {
        OnClick?.Invoke(this);
        Use();
    }

    public virtual void ToggleSelected(bool isSelected)
    {
        this.isSelected = isSelected;
        if (isSelected) button.Select();
    }

    public abstract void Use();
}
