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

    // ================= [NEW] people interaction (used by PeopleAI) =================
    [SerializeField] private float personEventDuration = 4f;   // how long a person waits while the event plays
    private PeopleAI claimedBy;                                 // the person currently heading to / using this tree
    private float cooldownUntil;                                // tree can't be targeted again before this time

    public float PersonEventDuration => personEventDuration;
    public bool IsAvailableForPeople => claimedBy == null && Time.time >= cooldownUntil;

    public bool TryClaim(PeopleAI person)
    {
        if (!IsAvailableForPeople) return false;
        claimedBy = person;
        return true;
    }

    public void ReleaseClaim(PeopleAI person, float cooldown)
    {
        if (claimedBy != person) return;
        claimedBy = null;
        cooldownUntil = Time.time + cooldown;
    }
    // ================= [END NEW] =================

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

    // [CHANGED] now takes an optional person.
    //   person == null  -> triggered by NatureManager (same as before, its call still compiles)
    //   person != null  -> triggered by a person walking up to this tree
    public void TriggerEvent(int rnd, GameObject person = null)
    {
        Debug.Log(rnd);

        if (rnd == 0) EventOne(person);
        if (rnd == 1) Eventtwo(person);
        if (rnd == 2) Eventthree(person);
        // Example effects:
        // change color
        // double income
        // play particles
    }

    IEnumerator WaitTBeforeAE() // waittimeBeforeAfterEvent and during this time there will be event animations like fire or dead tree mesh then tree bye bye
    {
        yield return new WaitForSeconds(waitTime);
    }

    // [CHANGED] the three event methods now receive the person's GameObject.
    // "gameObject" is this tree, "person" is the character (null if NatureManager triggered it).
    // Play the tree animation and the person animation here.
    void EventOne(GameObject person) // fire
    {
        Debug.Log("set trees on fire");
        StartCoroutine(WaitTBeforeAE());
    }

    void Eventtwo(GameObject person) // draught
    {
        Debug.Log("set trees to dry");
        StartCoroutine(WaitTBeforeAE());
    }

    void Eventthree(GameObject person) // storm
    {
        Debug.Log("break trees");
        StartCoroutine(WaitTBeforeAE());
    }
}