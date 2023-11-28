using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public int startingDiceIndex = 0;
    public GameObject[] dices;
    public int diceIndex;
    [SerializeField] Transform diceSpawnPoint;

    void Start()
    {
        diceIndex = startingDiceIndex;
        SetActiveDice(diceIndex);
    }

    public void SetActiveDice(int index)
    {
        if(index >= dices.Length || index <0)
        {
            Debug.LogWarning("Tride to switch to a Dice that does not exist. Make sure you have all the correcct Dices in your dice array");
        }

        diceIndex = index;
        for (int i = 0; i < dices.Length; i++)
        {
            dices[i].SetActive(false);
        }

        dices[index].SetActive(true);
        dices[index].transform.position = diceSpawnPoint.transform.position;
        dices[index].transform.rotation = diceSpawnPoint.transform.rotation;
        dices[index].GetComponent<Rigidbody>().velocity = Vector3.zero;
        dices[index].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        dices[index].GetComponent<Roll>().isRolling = false;
    }
}
    