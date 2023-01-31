using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellScript : MonoBehaviour
{
    //Get Game Manager
    private GameManager gameManager;

    //Get Spell Queue Panel
    private GameObject spellQueuePanel;

    //Get Queue Spell Prefab
    [SerializeField] private GameObject queueSpellPrefab;

    //Spell Owner
    private GameObject spellOwner;

    //Spell Target
    private GameObject spellTarget; //make this a list when you need multiple enemies at once

    //-------Spell Effects-------
    private bool dealsDamage;
    private float damageAmount;

    //-------Spell Stats-------
    private float spellManaCost;
    private float spellCooldown;
    private float spellMaxUsesPerCombat;
    private float spellMaxUsesPerGame;
    private string spellType;

    //-------Properties-------
    //Owner and Target Properties
    public GameObject SpellOwner
    {
        get { return spellOwner; }
        set { spellOwner = value; }
    }

    public GameObject SpellTarget
    {
        get { return spellTarget; }
        set { spellTarget = value; }
    }

    //Spell Effects Properties
    public bool DealsDamage
    {
        get { return dealsDamage; }
        set { dealsDamage = value; }
    }

    public float DamageAmount
    {
        get { return damageAmount; }
        set { damageAmount = value; }
    }

    //Spell Stats Properties
    public float SpellManaCost
    {
        get { return spellManaCost; }
        set { spellManaCost = value; }
    }

    public float SpellCooldown
    {
        get { return spellCooldown; }
        set { spellCooldown = value; }
    }

    public float SpellMaxUsesPerCombat
    {
        get { return spellMaxUsesPerCombat; }
        set { spellMaxUsesPerCombat = value; }
    }

    public float SpellMaxUsesPerGame
    {
        get { return spellMaxUsesPerGame; }
        set { spellMaxUsesPerGame = value; }
    }

    public string SpellType
    {
        get { return spellType; }
        set { spellType = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get Game Manager
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Get Spell Queue Panel
        spellQueuePanel = GameObject.Find("Spell Queue Panel");
    }

    //Put Spell On Queue
    public void AddSpellToQueue()
    {
        //Check for open slot in spell queue
        int filledQueueSlots = 0;

        for (int i = 0; i < gameManager.SpellQueueLength; i++)
        {
            if (gameManager.spellQueueList[i] != null)
            {
                filledQueueSlots++;
            }
        }

        //If a slot is open...
        if (filledQueueSlots < gameManager.SpellQueueLength)
        {
            //Add spell to queue
            gameManager.spellQueueList[filledQueueSlots] = gameObject;
            var newQueueSpell = Instantiate(queueSpellPrefab, spellQueuePanel.transform);

            //Add Icon to queue spell
            newQueueSpell.GetComponent<Image>().sprite = transform.Find("Spell Sprite").GetComponent<Image>().sprite;
        }
    }

    //Activate the spell's effect (when first in the queue)
    public void ActivateSpell()
    {

    }
}