using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Task1
{
    public class SceneView : MonoBehaviour
    {
        [SerializeField] private Image webImg;
        [SerializeField] private Image resourceImg;
        [SerializeField] private Slider webSlider;
        [SerializeField] private Slider resourceSlider;
        [SerializeField] private Slider sceneSlider;
        [SerializeField] private TextMeshProUGUI webText;
        [SerializeField] private TextMeshProUGUI resourceText;
        [SerializeField] private TextMeshProUGUI sceneText;
        [SerializeField] private Button sceneBtn;

        public Button SceneBtn => sceneBtn;

        public void SetWebImage(Sprite img) =>
            webImg.sprite = img;

        public void SetWebProgress(float value)
        {
            webSlider.value = value;
            webText.text = $"Загрузка(Web): {value*100:F0}/100%";
        }

        public void SetResourceImage(Sprite img) =>
            resourceImg.sprite = img;

        public void SetResourceProgress(float value)
        {
            resourceSlider.value = value;
            resourceText.text = $"Загрузка(Resource): {value * 100:F0}/100%";
        }

        public void SetSceneProgress(float value)
        {
            sceneText.text = $"Next scene: {value * 100:F0}/100%";
            sceneSlider.value = value;
        }
    }
}