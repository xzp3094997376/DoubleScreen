using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DepthUIDebug : MonoBehaviour
{
    public GetCameraDepth depth;
    public float depthScale = 1;
    public Text scaleText;
    public Slider Slider;
    public Text SliderValue;
    public float maxNearMove;
    private float NearMove;
    public Text NearMoveText;
    // Use this for initialization
    void Start()
    {
        Slider.maxValue = maxNearMove;
        Slider.minValue = -maxNearMove;
        Slider.onValueChanged.AddListener(OnSliderValueChange);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.LeftArrow))
        //{
        //    depthScale -= Slider.value;
        //    depth.ReplaceDepthScale(depthScale);
        //    scaleText.text = depthScale.ToString("0.00");
        //}
        //if (Input.GetKey(KeyCode.RightArrow))
        //{
        //    depthScale += Slider.value;
        //    depth.ReplaceDepthScale(depthScale);
        //    scaleText.text = depthScale.ToString("0.00");
        //}
        //if (Input.GetKey(KeyCode.UpArrow))
        //{
        //    NearMove += Slider.value;
        //    depth.ReplaceDepthCameraNearMove(NearMove);
        //    NearMoveText.text = NearMove.ToString("0.00");
        //}
        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    NearMove -= Slider.value;
        //    depth.ReplaceDepthCameraNearMove(NearMove);
        //    NearMoveText.text = NearMove.ToString("0.00");
        //}
    }
    public void OnSliderValueChange(float value)
    {
        SliderValue.text = Slider.value.ToString();
        //Debug.Log(value);
        //Debug.Log(Slider.value);
        //depth.ReplaceDepthCameraNearMove(Slider.value);
        //NearMove = Slider.value;
    }
}
