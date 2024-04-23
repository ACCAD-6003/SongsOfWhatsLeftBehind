using System;

public interface IButton
{
    event Action<IButton> OnSelect;
    void ToggleSelected(bool isSelected);
}
