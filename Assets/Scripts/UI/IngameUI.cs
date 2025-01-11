using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngameUI : MonoBehaviour
{
    public Slider HPSlider;  // 슬라이더 연결
    public Slider MPSlider;  // 슬라이더 연결
    public TMP_Text timeText;
    string minutes;
    string seconds;
    
    void Start()
    {
        // 슬라이더 초기값 설정
        HPSlider.minValue = 0f;
        HPSlider.maxValue = 100f;
        HPSlider.value = 100f;
        MPSlider.minValue = 0f;
        MPSlider.maxValue = 100f;
        MPSlider.value = 100f;

        // 슬라이더 값 변경 이벤트 연결
        //HPSlider.onValueChanged.AddListener(OnHPSliderValueChanged);
        //MPSlider.onValueChanged.AddListener(OnMPSliderValueChanged);
    }

    void OnHPSliderValueChanged(float value)
    {
        HPSlider.value = GameManager.Instance.playerHP;
    }

    void Update()
    {
        HPSlider.value = GameManager.Instance.playerHP;
        MPSlider.value = GameManager.Instance.playerMP;

        LeftTime();
    }

    private void LeftTime()
    {
        minutes = (GameManager.Instance.leftTime / 60).ToString();
        seconds = (GameManager.Instance.leftTime % 60).ToString();
        if ((GameManager.Instance.leftTime % 60) < 10f) seconds = "0" + seconds;
        timeText.text = minutes + ":" + seconds;
    }
}
