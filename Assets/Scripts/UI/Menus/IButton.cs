using System;

public interface IButton
{
    event Action<IButton> OnSelect;
    void Click();
    void ToggleSelected(bool isSelected);
    void Use();
}
