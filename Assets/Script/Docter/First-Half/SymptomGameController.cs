using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SymptomGameController : MonoBehaviour
{
    [Header("Symptoms Settings")]
    [SerializeField, Tooltip("List of all available symptoms.")]
    private List<Symptom> symptomsList;

    [SerializeField, Tooltip("First fixed symptom that always appears.")]
    private Symptom fixedSymptom1;

    [SerializeField, Tooltip("Second fixed symptom that always appears.")]
    private Symptom fixedSymptom2;

    private Symptom randomSymptom;
    private Symptom currentSymptom;
    private List<Symptom> activeSymptoms = new List<Symptom>();

    [Header("UI Elements")]
    [SerializeField, Tooltip("Text element to display the current symptom prompt.")]
    private TMP_Text symptomPromptText;

    [SerializeField] private GameObject talkObject;

    [SerializeField, Tooltip("Checkboxes for each symptom.")]
    private List<Toggle> symptomCheckboxes;

    [SerializeField, Tooltip("Layout group to control the symptom checkbox arrangement.")]
    private VerticalLayoutGroup layoutGroup;

    private void Awake()
    {
        UITransitionUtility.Instance.Initialize(talkObject,Vector2.zero);
    }

    private void Start()
    {
        //GameManager.Instance.SetScoreTextActive(false);
        SetupSymptoms();
        SetRandomSymptom();
        ShuffleToggles();
        foreach (Toggle toggle in symptomCheckboxes)
        {
            toggle.onValueChanged.AddListener(delegate { CheckAnswer(toggle); });
        }
    }
    
    private void SetupSymptoms()
    {
        activeSymptoms = new List<Symptom> { fixedSymptom1, fixedSymptom2 };
        
        List<Symptom> availableSymptoms = new List<Symptom>(symptomsList);
        availableSymptoms.Remove(fixedSymptom1);
        availableSymptoms.Remove(fixedSymptom2);

        if (availableSymptoms.Count > 0)
        {
            randomSymptom = availableSymptoms[Random.Range(0, availableSymptoms.Count)];
            activeSymptoms.Add(randomSymptom);
        }
    }

    private void SetRandomSymptom()
    {
        if (activeSymptoms.Count > 0)
        {
            currentSymptom = activeSymptoms[Random.Range(0, activeSymptoms.Count)];
            StartCoroutine(PopTextObject());
        }
        else
        {
            GameManager.Instance.healthManager.WinGame();
            ClearToggleListeners();
        }
    }
    
    private void ShuffleToggles()
    {
        List<Toggle> shuffledToggles = new List<Toggle>(symptomCheckboxes);
        for (int i = 0; i < shuffledToggles.Count; i++)
        {
            Toggle temp = shuffledToggles[i];
            int randomIndex = Random.Range(i, shuffledToggles.Count);
            shuffledToggles[i] = shuffledToggles[randomIndex];
            shuffledToggles[randomIndex] = temp;
        }
        
        foreach (Toggle toggle in shuffledToggles)
        {
            toggle.transform.SetParent(null); 
        }
        foreach (Toggle toggle in shuffledToggles)
        {
            toggle.transform.SetParent(layoutGroup.transform);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
    }

    private void CheckAnswer(Toggle selectedToggle)
    {
        if (!selectedToggle.isOn) return;
        
        TMP_Text toggleLabel = selectedToggle.GetComponentInChildren<TMP_Text>();
        if (toggleLabel == null) return;

        if (toggleLabel.text == currentSymptom.symptomDescription)
        {
            activeSymptoms.Remove(currentSymptom);
            selectedToggle.interactable = false;
            UITransitionUtility.Instance.PopDown(talkObject);
            SetRandomSymptom();
        }
        else
        {
            GameManager.Instance.healthManager.DecreaseHealth(1);
            selectedToggle.isOn = false;
        }
        SoundManager.PlaySound(SoundType.CheckBox,VolumeType.SFX);
    }
    
    private void ClearToggleListeners()
    {
        foreach (Toggle toggle in symptomCheckboxes)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }

    private IEnumerator PopTextObject()
    {
        yield return new WaitForSeconds(0.5f);
        symptomPromptText.text = currentSymptom.symptomDescription;
        UITransitionUtility.Instance.PopUp(talkObject);
    }
}
