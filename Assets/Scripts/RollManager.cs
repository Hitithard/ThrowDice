using UnityEngine;
using TMPro;

public class RollManager : MonoBehaviour
{
    public Transform boundingBoxMin;    //Set the first corner of Dice boundary
    public Transform boundingBoxMax;    //Set the secound corner of Dice boundary to create box where Dice can roll

    public TextMeshProUGUI resultText;      // Assign this to your "Result:" text field in the Unity inspector.
    public TextMeshProUGUI totalText;       // Assign this to your "Total:" text field in the Unity inspector.

    public TextMeshPro resultText2;      // Assign this to your "Result:" text field in the Unity inspector.
    public TextMeshPro totalText2;       // Assign this to your "Total:" text field in the Unity inspector.

    public int totalValue = 0;
    public bool rollRotation = false;
    public bool dragRoll = false;

    public float minPowerToRoll = 1f;   // The minimum power needed to conider throw as a roll 
    public float throwPowerDivision = 20f; 
    void Start()
    {
        ResetTotal();
    }
    public void ResetTotal()
    {
        totalValue = 0;
        totalText.text = ("Total:0");
        totalText2.text = ("Total:0");

    }
    public void Rolling()
    {
        resultText.text = ("Result:?");
        resultText2.text = ("Result:?");

    }

    // Update the "Result:" and "Total:" texts based on the roll result
    public void RollResult(int result)
    {
        resultText.text = ("Result:" + result.ToString());
        resultText2.text = ("Result:" + result.ToString());


        totalValue += result;
        totalText.text = ("Total:" + totalValue.ToString());
        totalText2.text = ("Total:" + totalValue.ToString());


    }

    // Automatically roll the dice
    public void AutoRoll()
    {
        DiceManager diceScript = GetComponent<DiceManager>();
        diceScript.dices[diceScript.diceIndex].GetComponent<Roll>().AutoRollDice();
    }

    public void ToggleRollRotation()
    {
        rollRotation = !rollRotation;
    }
    public void ToggleDragRoll()
    {
        dragRoll = !dragRoll;
    }

}
