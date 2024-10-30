using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SymptomGameController : MonoBehaviour
{
    [Header("Symptoms")]
    public List<Symptom> symptomsList;
    private Symptom currentSymptom;

    [Header("UI Elements")]
    public TMP_Text symptomPromptText; // Text to display the current symptom
    public List<Toggle> symptomCheckboxes; // Checkboxes for each symptom
    public VerticalLayoutGroup layoutGroup;

    private void Start()
    {
        GameManager.Instance.SetScoreTextActive(false);
        SetRandomSymptom();
        ShuffleToggles();

        // Assign CheckAnswer() to each checkbox's OnValueChanged event
        foreach (Toggle toggle in symptomCheckboxes)
        {
            toggle.onValueChanged.AddListener(delegate { CheckAnswer(toggle); });
        }
    }

    private void SetRandomSymptom()
    {
        if (symptomsList.Count > 0)
        {
            currentSymptom = symptomsList[Random.Range(0, symptomsList.Count)];
            symptomPromptText.text = currentSymptom.symptomDescription;
        }
        else
        {
            GameManager.Instance.WinGame();
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
            symptomsList.Remove(currentSymptom);
            SetRandomSymptom();
        }
        else
        {
            GameManager.Instance.DecreaseHealth(1);
            selectedToggle.isOn = false;
        }
    }
}
