using System;
using UnityEngine;
using UnityEngine.UI; // ✅ Use this, not UIElements

public class Wood : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider goldSlider;
    [SerializeField] private Slider workerSlider;

    [Header("Costs & Results")]
    public int goldCost;
    public int workerCost;
    public float timeCost;

    [Header("Settings")]
    [SerializeField] private Vector2 goldSliderConstraints;
    [SerializeField] private Vector2 workerSliderConstraints;
    [SerializeField] private int stepSize = 1;
    [SerializeField] private WoodType type;

    private void Start()
    {
        SetupSlider(goldSlider, goldSliderConstraints);
        SetupSlider(workerSlider, workerSliderConstraints);

        goldSlider.onValueChanged.AddListener(_ => UpdateValues());
        workerSlider.onValueChanged.AddListener(_ => UpdateValues());

        UpdateValues();
    }

    private void SetupSlider(Slider slider, Vector2 constraints)
    {
        int min = Mathf.RoundToInt(constraints.x);
        int max = Mathf.RoundToInt(constraints.y);

        min = Mathf.CeilToInt((float)min / stepSize) * stepSize;
        max = Mathf.FloorToInt((float)max / stepSize) * stepSize;

        if (min > max) min = max = Mathf.RoundToInt(constraints.x);

        slider.wholeNumbers = true;
        slider.minValue = min;
        slider.maxValue = max;
        slider.SetValueWithoutNotify(min);
    }

    private void UpdateValues()
    {
        goldCost = Mathf.RoundToInt(goldSlider.value / stepSize) * stepSize;
        workerCost = Mathf.RoundToInt(workerSlider.value / stepSize) * stepSize;

        WoodData data = GameManager.instance.woodDataList.Find(w => w.type == type);

        if (data != null)
        {
            // zTime = (workers * gold) / timeToChop * amount
            // assuming amount = 1 for now (can be parameterized)
            timeCost = (workerCost * goldCost) / (float)data.timeToChop;

            timeCost = TimeAdjustmentScript.LogReduce(timeCost);
        }
        else
        {
            timeCost = 0f;
        }

        Debug.Log($"Wood: {type}, Gold: {goldCost}, Workers: {workerCost}, Time: {timeCost}");
    }

    public WoodType AssignWoodType(string _name)
    {
        if (Enum.TryParse(_name, true, out WoodType result))
            return result;

        return WoodType.Oak; // default fallback
    }
}

[Serializable]
public class WoodData
{
    public WoodType type;
    public int purchasePrice;
    public int timeToChop;
}

public enum WoodType
{
    Oak,
    Birch,
}
