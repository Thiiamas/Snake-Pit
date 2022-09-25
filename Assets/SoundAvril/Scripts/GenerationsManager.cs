using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerationsManager : MonoBehaviour
{
    public Dropdown _dropdown;
    public Slider _sliderSize;
    public Toggle _outward;
    public KochSphere _kSphere;
    public Button _AddGeneration;
    public Button _RemoveGeneration;

    int _currentIndex;
    // Start is called before the first frame update
    void Start()
    {
        _dropdown.options.Clear();
        _dropdown.onValueChanged.AddListener(delegate
        {
            DropdownItemSelected(_dropdown);
        });
        _outward.onValueChanged.AddListener(delegate
        {
            OutwardToggleChanged();
        });
        _AddGeneration.onClick.AddListener(delegate
        {
            AddGenerationClicked();
        });
        _RemoveGeneration.onClick.AddListener(delegate
        {
            RemoveGenerationClicked();
        });
        _sliderSize.minValue = -3;
        _sliderSize.maxValue = 3;
        _sliderSize.onValueChanged.AddListener(delegate { ValueChanged(); });
        _sliderSize.value = _kSphere._startGen[0].scale;
    }

    void AddGenerationClicked()
    {
        _kSphere.AddGeneration();
        
    }

    void RemoveGenerationClicked()
    {
        _kSphere.RemoveGeneration();
    }
    void OutwardToggleChanged()
    {
        _kSphere._startGen[_dropdown.value].outwards = _outward.isOn;
    }

    public void AddItem(List<int> gens)
    {
        int count = 0;
        _dropdown.options.Clear();
        foreach(int name in gens)
        {
            count++;
            _dropdown.options.Add(new Dropdown.OptionData()
            {
                text = name.ToString()
            });
        }
        _dropdown.value = count - 1;
        _currentIndex = count - 1;
        if (_kSphere._startGen.Length > 0)
        {
            _sliderSize.value = _kSphere._startGen[_currentIndex].scale;
            _outward.SetIsOnWithoutNotify(_kSphere._startGen[_currentIndex].outwards);
        }


    }

    public void RemoveItem(int index)
    {
        _dropdown.options.Remove(_dropdown.options.Find(x => x.text == index.ToString()));
    }

    void DropdownItemSelected(Dropdown dropdown)
    {
        if (_kSphere._startGen.Length == 0)
        {
            return;
        }
        int index = dropdown.value;
        _currentIndex = index;
        _sliderSize.value = _kSphere._startGen[index].scale;


    }

    void ValueChanged()
    {
        _kSphere._startGen[_currentIndex].scale = _sliderSize.value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
