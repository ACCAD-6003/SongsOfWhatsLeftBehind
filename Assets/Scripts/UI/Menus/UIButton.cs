using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public abstract class UIButton : MonoBehaviour, IButton
{
    public event Action<IButton> OnSelect;
    public event Action<IButton> OnClick;
    private Button button;
    
    public virtual void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Click);
    }

    public void Click()
    {
        OnClick?.Invoke(this);
        Use();
    }

    public virtual void ToggleSelected(bool isSelected)
    {
        button.Select();
    }

    public abstract void Use();
}
