using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialCursor : MonoBehaviour
{
    [Header("Tutorial UI")]
    public TextMeshProUGUI tutorialText;
    public Button continueButton;
    public Button handModeToggleButton;

    [Header("References")]
    public UnifiedCursor unifiedCursor;

    private int currentStep = 0;
    private List<TutorialStep> steps = new List<TutorialStep>();
    private bool isWaitingForInteraction = false;

    // Interaction tracking
    private bool unitSelected = false;
    private bool attacked = false;
    private bool moved = false;
    private bool built = false;
    private bool building = false;
    private bool unitHired = false;
    public Unit Enemy;

    [System.Serializable]
    public class TutorialStep
    {
        public string instructionText;
        public bool allowImmediateContinue = false;
        public GameObject requiredClickTarget = null;
        public bool requireUnitSelection = false;
        public bool requireAttack = false;
        public bool requireMove = false;
        public bool requireBuild = false;
        public bool requireBuilding = false;
        public bool requireHire = false;
    }

    void Awake()
    {
        continueButton.onClick.AddListener(VerifyBeforeContinuing);
        handModeToggleButton.onClick.AddListener(ToggleInputMode);
        SetupTutorialSteps();
        
    }

    public void ResetTutorial()
    {
        currentStep = 0;
        unitSelected = false;
        attacked = false;
        moved = false;
        built = false;
        building = false; 
        unitHired = false;
    }
    private void Start()
    {
        Enemy = FindAnyObjectByType<Unit>();
        Enemy.gameObject.SetActive(false);
    }
    void SetupTutorialSteps()
    {
        steps = new List<TutorialStep>
    {
        // Step 0: Introduction
        new TutorialStep {
            instructionText = "",
            allowImmediateContinue = true
        },

        // Step 1: Initial FOB Selection
        new TutorialStep {
            instructionText = "The first thing you must do in every match is hire a Unit.\nLEFT-CLICK your F.O.B. (Forward Operating Base) to select it.",
            requireBuild = true
        },

        // Step 2: Resource Explanation
        new TutorialStep {
            instructionText = "To build your army you have two resources:\n" +
                             "- UNIT TOKENS: Used to hire troops (gained from Barracks)\n" +
                             "- MATERIALS: Used to create buildings (gained from Factories)\n" +
                             "These are shown at the top of your HUD.",
            allowImmediateContinue = true
        },

        // Step 3: First Unit Hire
        new TutorialStep {
            instructionText = "Now hire your first troop:\n" +
                             "1. Click Spawn Troop in the building menu\n" +
                             "2. Select a unit type\n" +
                             "3. Click HIRE",
            requireHire = true
        },

        // Step 4: Unit Production Wait
        new TutorialStep {
            instructionText = "Good job! Your unit is now being created (shown above your F.O.B.).\n" +
                             "It will be ready after the displayed number of turns.\n" +
                             "Click END TURN to continue.",
            allowImmediateContinue = true
        },

        // Step 5: Turn Transition
        new TutorialStep {
            instructionText = "For the sake of this tutorial, the enemy turn will pass immediately.",
            allowImmediateContinue = true
        },

        // Step 6: Unit Movement
        new TutorialStep {
            instructionText = "Your unit is now ready! Select it and:\n" +
                             "1. Click MOVE\n" +
                             "2. LEFT-CLICK a highlighted tile",
            requireMove = true
        },

        // Step 7: Building Construction
        new TutorialStep {
            instructionText = "Now let's build something:\n" +
                             "1. Make sure you're on an empty tile\n" +
                             "2. Click BUILD\n" +
                             "3. Select a structure from the menu\n" +
                             "4. Confirm placement\n" +
                             "5. Select Your new Building",
            requireBuilding = true,
        },

        new TutorialStep {
            instructionText = "In the building menu, find the unit in the Stored Units section and select it to bring it back out.",
            allowImmediateContinue = true
        },

        // Step 8: Combat Introduction
        new TutorialStep {            
            instructionText = "Oh no! Theres an enemy at your FOB!\n" +
                             "To attack enemies:\n" +
                             "1. Select your unit\n" +
                             "2. Click ATTACK\n" +
                             "3. LEFT-CLICK an enemy in range",
            requireAttack = true
        },

        // Step 9: Advanced Building
        new TutorialStep {
            instructionText = "Remember:\n" +
                             "- Build Barracks to get more Unit Tokens\n" +
                             "- Build Factories to get more Materials\n" +
                             "- Different buildings enable different units",
            allowImmediateContinue = true
        },

        // Step 10: Completion
        new TutorialStep {
            instructionText = "Tutorial complete! You now know:\n" +
                             "- How to hire and move units\n" +
                             "- How to construct buildings\n" +
                             "- How to attack enemies\n" +
                             "- NOTE: in game if you would like to use hand tracking press T\n" +
                             "  This will alternate the input mode" +
                             "Click CONTINUE to begin your mission!",
            allowImmediateContinue = true
        }
    };
    }

    public void SetStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= steps.Count) return;

        currentStep = stepIndex;
        TutorialStep step = steps[stepIndex];
        tutorialText.text = step.instructionText;

        // Reset interaction flags for the new step
        unitSelected = false;
        attacked = false;
        moved = false;
        built = false;
        building = false;
        unitHired = false;

        // Show/hide hand tracking button only when relevant
        handModeToggleButton.gameObject.SetActive(stepIndex == 6);

        // Handle continue button state
        continueButton.interactable = step.allowImmediateContinue;
        isWaitingForInteraction = !step.allowImmediateContinue;

    }

    void VerifyBeforeContinuing()
    {
        if (!isWaitingForInteraction)
        {
            NextStep();
            return;
        }

        TutorialStep current = steps[currentStep];
        bool requirementsMet = true;

        if (current.requireUnitSelection) requirementsMet &= unitSelected;
        if (current.requireAttack) requirementsMet &= attacked;
        if (current.requireMove) requirementsMet &= moved;
        if (current.requireBuild) requirementsMet &= built;
        if (current.requireBuilding) requirementsMet &= built;
        if (current.requireHire) requirementsMet &= unitHired;
        {
            
        }
        {
            
        }

        if (requirementsMet)
        {
            NextStep();
        }
    }

    public void NextStep()
    {
        if (currentStep + 1 >= steps.Count)
        {
            EndTutorial();
        }        
        else
        {
            SetStep(currentStep + 1);
        }
        if (currentStep == 8)
        {
            Enemy.gameObject.SetActive(true);
        }
    }

    // Interaction notification methods
    public void NotifyUnitSelected()
    {
        if (!isWaitingForInteraction) return;
        unitSelected = true;
        Debug.Log("Unit selected - checking if can continue");
        VerifyBeforeContinuing();
    }
    public void NotifyUnitHired()
    {
        if (!isWaitingForInteraction) return;
        unitHired = true;
        Debug.Log("Unit Hired - checking if can continue");
        VerifyBeforeContinuing();
    }

    public void NotifyAttackPerformed()
    {
        if (!isWaitingForInteraction) return;
        attacked = true;
        Debug.Log("Attack performed - checking if can continue");
        VerifyBeforeContinuing();
    }

    public void NotifyMovePerformed()
    {
        if (!isWaitingForInteraction) return;
        moved = true;
        Debug.Log("Move performed - checking if can continue");
        VerifyBeforeContinuing();
    }

    public void NotifyBuildPerformed()
    {
        if (!isWaitingForInteraction) return;
        built = true;
        Debug.Log("Build performed - checking if can continue");
        VerifyBeforeContinuing();
    }
    public void NotifyBuildingCreate()
    {
        if (!isWaitingForInteraction) return;
        building = true;
        Debug.Log("Building Created - checking if can continue");
        VerifyBeforeContinuing();
    }

    public void ToggleInputMode()
    {
        UnifiedCursor.GlobalInputMode = (UnifiedCursor.GlobalInputMode == UnifiedCursor.InputMode.Mouse)
            ? UnifiedCursor.InputMode.HandTracking
            : UnifiedCursor.InputMode.Mouse;

        if (unifiedCursor != null)
        {
            unifiedCursor.ClearAll();
        }
    }

    public int StepsCount()
    {
        return steps.Count;
    }

    public void EndTutorial()
    {
        tutorialText.text = "Tutorial Complete!\nYou may now explore freely.";
        continueButton.gameObject.SetActive(false);
        handModeToggleButton.gameObject.SetActive(false);
        enabled = false;
        SceneManager.LoadScene(0);
    }
}