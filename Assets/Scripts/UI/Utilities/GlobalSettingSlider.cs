using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class GlobalSettingSlider : MonoBehaviour
{
    [SerializeField] GlobalSetting setting;

    private void OnEnable()
    {
        GetComponent<Slider>().value = setting.Value;
    }

    public void OnSliderChange(float value)
    {
        setting.Value = (int)value;
    }
}
