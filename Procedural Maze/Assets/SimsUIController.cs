using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimsUIController : MonoBehaviour {

    [SerializeField] private InputField FirstName;
    [SerializeField] private InputField LastName;

    [SerializeField] private Slider Shy_Outgoing_slider;
    [SerializeField] private Slider Sloppy_Nest_slider;
    [SerializeField] private Slider Lazy_Active_slider;
    [SerializeField] private Slider Serious_Playful_slider;
    [SerializeField] private Slider Grouchy_Nice_slider;

    [SerializeField] private SimsTextureCreator simsTextureCreator;

    public void OnGenerateButtonClicked()
    {
        simsTextureCreator.GenerateTexture(FirstName.text, LastName.text, 
            Mathf.RoundToInt(Shy_Outgoing_slider.value),
            Mathf.RoundToInt(Sloppy_Nest_slider.value),
            Mathf.RoundToInt(Lazy_Active_slider.value),
            Mathf.RoundToInt(Serious_Playful_slider.value),
            Mathf.RoundToInt(Grouchy_Nice_slider.value));
    }
}
