using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField, Tooltip("Checkboxes for each symptom.")]
    private List<Toggle> symptomCheckboxes;

    [SerializeField, Tooltip("Layout group to control the symptom checkbox arrangement.")]
    private VerticalLayoutGroup layoutGroup;


    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
        SetupSymptoms();
        SetRandomSymptom();
        ShuffleToggles();

        // Assign CheckAnswer() to each checkbox's OnValueChanged event
        foreach (Toggle toggle in symptomCheckboxes)
        {
            toggle.onValueChanged.AddListener(delegate { CheckAnswer(toggle); });
        }
    }
    
    private void SetupSymptoms()
    {
        activeSymptoms = new List<Symptom> { fixedSymptom1, fixedSymptom2 };

        // Choose one random symptom that isn’t fixedSymptom1 or fixedSymptom2
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
            symptomPromptText.text = currentSymptom.symptomDescription;
        }
        else
        {
            GameManager.Instance.WinGame();
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

        // Remove current toggles from the layout group and re-add in shuffled order
        foreach (Toggle toggle in shuffledToggles)
        {
            toggle.transform.SetParent(null); // Temporarily remove from layout
        }
        foreach (Toggle toggle in shuffledToggles)
        {
            toggle.transform.SetParent(layoutGroup.transform); // Re-add in new order
        }

        // Refresh the layout group to apply the new order
        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
    }

    private void CheckAnswer(Toggle selectedToggle)
    {
        if (!selectedToggle.isOn) return;
        
        TMP_Text toggleLabel = selectedToggle.GetComponentInChildren<TMP_Text>();
        if (toggleLabel == null) return;

        if (toggleLabel.text == currentSymptom.symptomDescription)
        {
            activeSymptoms.Remove(currentSymptom); // Remove matched symptom
            selectedToggle.interactable = false; // Disable toggle
            SetRandomSymptom(); // Set the next symptom or end the game
        }
        else
        {
            GameManager.Instance.DecreaseHealth(1);
            selectedToggle.isOn = false;
        }
    }
    
    private void ClearToggleListeners()
    {
        foreach (Toggle toggle in symptomCheckboxes)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
}
