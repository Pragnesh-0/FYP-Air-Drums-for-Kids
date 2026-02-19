using UnityEngine;
using UnityEngine.UI;

public class StickSettings : MonoBehaviour
{

    public Slider betaSlider;
    public Slider minCutOff;
    public Slider reboundThreshold;

    public BetterDetection stick1;
    public BetterDetection stick2;
    public GameObject showcase;


    public Button showButton;
    public ComputerVisionDetection model;

    void Start()
    {
        betaSlider.onValueChanged.AddListener(delegate{changeBeta();});
        minCutOff.onValueChanged.AddListener(delegate{changeMinCutoff();});
        reboundThreshold.onValueChanged.AddListener(delegate{changeStickRebound();});
        showButton.onClick.AddListener(delegate{showcase.SetActive(!showcase.activeSelf); showCase(showcase.activeSelf);});
    }


    public void changeBeta()
    {
        model.changeBeta(betaSlider.value);
    }

    public void changeMinCutoff()
    {
        model.changeMinCutoff(minCutOff.value);
    }

    public void showCase(bool val)
    {
        model.cameraAction(val);
        if (!val)
        {
            showcase.SetActive(false);
        }
    }

    public void changeStickRebound()
    {
        float val = reboundThreshold.value;
        stick1.setReboundThreshold(val);
        stick2.setReboundThreshold(val);
    }


    public void setSettings(CalibrationSettings cs)
    {
        minCutOff.value = cs.noise;
        betaSlider.value = cs.lag;
        reboundThreshold.value = cs.reboundThreshold;

        changeBeta();
        changeMinCutoff();
        changeStickRebound();
    }

    public CalibrationSettings getSettings()
    {
        CalibrationSettings cs = new CalibrationSettings();
        cs.lag = betaSlider.value;
        cs.noise = minCutOff.value;
        cs.reboundThreshold = reboundThreshold.value;
        return cs;
    }
}
