using System.Collections;
using UnityEngine;
using TMPro;
// using UnityEngine.UI;

public class TreeBehaviour : MonoBehaviour
{
    // [SerializeField] TMP_Text moneyTxt;
    [SerializeField] 
    private float acaciaM = 5;
    private float bambooM;
    private float birchM;
    private float willowM;
    private float oakM;
    private float palmM;
    private float pineM;
    private float bushM;
    private float jungleM;
    private float poplarM;
    
    private float money;
    private float timeS = 3f;


    void Start()
    {
        if (CompareTag("Acacia"))
        {
            StartCoroutine(AddMoneyOverTime());
        }
    }

    IEnumerator AddMoneyOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeS); // wait 3 seconds
            MoneyManager.Instance.AddMoney(acaciaM);
        }
    }
    private void Update()
    {

    }
}
