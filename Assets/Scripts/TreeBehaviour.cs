using System.Collections;
using UnityEngine;

// using UnityEngine.UI;

public class TreeBehaviour : MonoBehaviour
{
    // [SerializeField] TMP_Text moneyTxt;
    [SerializeField] private float waitTime = 10f;
    private ObjectPlacer objectPlacer;
    private RemovingState removingState;
    private InputManager inputManager;
    // RemovingState removingState;
    private float acaciaM = 5;
    private float bambooM = 5;
    private float birchM = 5;
    private float willowM = 5;
    private float oakM = 3;
    private float palmM = 2;
    private float pineM = 7;
    private float bushM = 5;
    private float jungleM = 5;
    private float poplarM = 5;
    
    private float money;
    int index;
    private float timeS = 10f;

    // public enum NatureEventsType
    // {
    //     Fire,
    //     Draught,
    //     Storm,
    // }
    void Start()
    {
        NatureManager.Instance.AddTree(this);
        objectPlacer = GameObject.Find("ObjectPLacer").GetComponent<ObjectPlacer>();
        // removingState = GameObject.Find("RemovingState").GetComponent<RemovingState>();
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
        IncrementMoney();
    }

    IEnumerator AddMoneyOverTime(float amnt)
    {
        while (true)
        {
            yield return new WaitForSeconds(timeS); // wait 3 seconds
            MoneyManager.Instance.AddMoney(amnt);
        }
    }
    void IncrementMoney()
    {
        if (CompareTag("Acacia"))
        {
            StartCoroutine(AddMoneyOverTime(acaciaM));
        }
        if (CompareTag("Bamboo"))
        {
            StartCoroutine(AddMoneyOverTime(bambooM));
        }
        if (CompareTag("Oak"))
        {
            StartCoroutine(AddMoneyOverTime(oakM));
        }
        if (CompareTag("Willow"))
        {
            StartCoroutine(AddMoneyOverTime(willowM));
        }
        if (CompareTag("Poplar"))
        {
            StartCoroutine(AddMoneyOverTime(poplarM));
        }
        if (CompareTag("Bushtree"))
        {
            StartCoroutine(AddMoneyOverTime(bushM));
        }
        if (CompareTag("Jungletree"))
        {
            StartCoroutine(AddMoneyOverTime(jungleM));
        }
        if (CompareTag("Pine"))
        {
            StartCoroutine(AddMoneyOverTime(pineM));
        }
        if (CompareTag("Birch"))
        {
            StartCoroutine(AddMoneyOverTime(birchM));
        }
        if (CompareTag("Palm"))
        {
            StartCoroutine(AddMoneyOverTime(palmM));
        }
    }
    private void OnDestroy()
    {
        if (NatureManager.Instance != null)
            NatureManager.Instance.RemoveTree(this);
    }
    
    // public void SetIndex(int i)
    // {
    //     index = i;
    //     // Debug.Log(gameObject.name + " index: " + index);
    // }

    public void TriggerEvent(int rnd)
    {
        Debug.Log(rnd);
        // NatureEventsType rand = (NatureEventsType)Random.Range(0, 3); // converting int to enum label Fire - 0, Draught - 1 so on

        // switch(rand)
        // {
        //     case NatureEventsType.Fire:
        //         EventOne();
        //         break;
        //     case NatureEventsType.Draught:
        //         Eventtwo();
        //         break;
        //     case NatureEventsType.Storm:
        //         Eventthree();
        //         break;
            
        // } // can call rnd no. in nature scpt n varify using var in behav scpt 

        if(rnd == 0) EventOne();
        if(rnd == 1) Eventtwo();
        if(rnd == 2) Eventthree();
        // Example effects:
        // change color
        // double income
        // play particles
    }

    IEnumerator WaitTBeforeAE() // waittimeBeforeAfterEvent
    {
        yield return new WaitForSeconds(waitTime);

        // Destroy(gameObject);
        // objectPlacer.RemoveObjectAt(RemovingState.gameObjectIndex);
    }
    void EventOne() // fire
    {
        Debug.Log("set trees on fire");
        StartCoroutine(WaitTBeforeAE());
    }

    void Eventtwo() // draught
    {
        Debug.Log("set trees to dry");
        StartCoroutine(WaitTBeforeAE());
    }

    void Eventthree() // storm
    {
        Debug.Log("break trees");
        StartCoroutine(WaitTBeforeAE());
    }
    private void Update()
    {

    }
}
