using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TutorialCursor tutorialCursor;
    private int currentStep = 0;


    public void StartTutorial()
    {
        currentStep = 0;
        Debug.Log("Starting tutorial from step 0");
        tutorialCursor.ResetTutorial();
        tutorialCursor.SetStep(currentStep);
    }

    public void AdvanceStep(int step)
    {
        if (step >= tutorialCursor.StepsCount())
        {
            Debug.Log("Tutorial completed");
            tutorialCursor.EndTutorial();
            return;
        }

        currentStep = step;
        tutorialCursor.SetStep(currentStep);
    }

    public void NextStep()
    {
        AdvanceStep(currentStep + 1);
    }
}