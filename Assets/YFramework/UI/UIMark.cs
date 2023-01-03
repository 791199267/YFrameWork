using UnityEngine;
using UnityEngine.UI;

public interface IField
{
    /// <summary>
    /// 字段类型名称
    /// </summary>
    string FieldTypeName { get; }
    /// <summary>
    /// 字段名
    /// </summary>
    string FieldEntityName { get; }
}

public class UIMark : MonoBehaviour,IField
{
    public Transform Transform
    {
        get { return this.transform; }
    }

    private string CustomComponentName = "";


    public string FieldTypeName { get => ComponentName; }
    public string FieldEntityName { get => transform.name; }

    public string ComponentName
    {
        get
        {
            if (!string.IsNullOrEmpty(CustomComponentName))
                return CustomComponentName;
            if (null != GetComponent("SkeletonAnimation"))
                return "SkeletonAnimation";
            if (null != GetComponent<ScrollRect>())
                return "ScrollRect";
            if (null != GetComponent<InputField>())
                return "InputField";
            if (null != GetComponent<Text>())
                return "Text";
            if (null != GetComponent("TextMeshProUGUI"))
                return "TextMeshProUGUI";
            if (null != GetComponent<Button>())
                return "Button";
            if (null != GetComponent<RawImage>())
                return "RawImage";
            if (null != GetComponent<Toggle>())
                return "Toggle";
            if (null != GetComponent<Slider>())
                return "Slider";
            if (null != GetComponent<Scrollbar>())
                return "Scrollbar";
            if (null != GetComponent<Image>())
                return "Image";
            if (null != GetComponent<ToggleGroup>())
                return "ToggleGroup";
            if (null != GetComponent<Animator>())
                return "Animator";
            if (null != GetComponent<Canvas>())
                return "Canvas";
            if (null != GetComponent("Empty4Raycast"))
                return "Empty4Raycast";
            if (null != GetComponent<RectTransform>())
                return "RectTransform";

            return "Transform";
        }
    }

}
