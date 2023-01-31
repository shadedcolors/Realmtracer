using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Progress Bars
    [SerializeField] private Image healthSphere;
    [SerializeField] private Image manaSphere;
    [SerializeField] private Image spellQueueTimer;

    //Prefabs
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spellPrefab;

    //Panels
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] private GameObject spellInventoryPanel;

    //All Spell Icons
    [SerializeField] private Sprite[] allSpellImages;


    //-------SPELL QUEUE STUFF-------
    //Spell Queue List
    public GameObject[] spellQueueList;

    //Spell Queue Length
    private int spellQueueLength = 5;

    //Spell Queue Timer
    private float spellQueueTimeAmount = 10.0f;

    //Empty Queue check
    private bool isQueueEmpty;

    //-------Player Stats-------


    //-------Properties-------
    public int SpellQueueLength
    {
        get { return spellQueueLength; }
        set { spellQueueLength = value; }
    }

    public float SpellQueueTimeAmount
    {
        get { return spellQueueTimeAmount; }
        set { spellQueueTimeAmount = value; }
    }

    private void Start()
    {
        //Set Spell Queue size at start of a battle...
        spellQueueList = new GameObject[spellQueueLength]; //CHANGE THIS LATER FOR EACH BATTLE using spellQueueLength (OR FOR DIFFERENT WAYS THE QUEUE CAN GROW) !
    }

    // Update is called once per frame
    void Update()
    {
        //-------Progress Bar Tests-------
        healthSphere.fillAmount -= 0.00024f;
        manaSphere.fillAmount -= 0.0001f;

        spellQueueTimer.fillAmount -= 1.0f / spellQueueTimeAmount * Time.deltaTime;

        //Activate Spell after spell queue progress bar ends
        if (spellQueueTimer.fillAmount <= 0)
        {
            //spellQueueList[0] = null;

            //Reset spell queue progress bar
            spellQueueTimer.fillAmount = 1;
        }

        //-------Player Spell Creation Test-------
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CreateSpell(spellInventoryPanel);
        }

        //-------Enemy Creation Test-------
        if (Input.GetKeyDown(KeyCode.E))
        {
            var newEnemy = Instantiate(enemyPrefab, enemyPanel.transform);
        }
    }
    
    //Create Spell Test
    public void CreateSpell(GameObject panel)
    {
        //Create New Spell
        var newSpell = Instantiate(spellPrefab, panel.transform);

        //Set Spell Icon
        newSpell.transform.Find("Spell Sprite").GetComponent<Image>().sprite = allSpellImages[Random.Range(0, allSpellImages.Length)]; //Change this to a set value later!

        //Set Spell Owner
        newSpell.GetComponent<SpellScript>().SpellOwner = gameObject;

        //Set Spell Target
        newSpell.GetComponent<SpellScript>().SpellTarget = null; //Add this later!

        //Set Spell Effect - Damage
        newSpell.GetComponent<SpellScript>().DealsDamage = true;
        newSpell.GetComponent<SpellScript>().DamageAmount = 1;

        //Set Spell Mana Cost
        newSpell.GetComponent<SpellScript>().SpellManaCost = 1;

        //Set Spell Cooldown
        newSpell.GetComponent<SpellScript>().SpellCooldown = 1;

        //Set Spell Max Uses Per Combat
        newSpell.GetComponent<SpellScript>().SpellMaxUsesPerCombat = -1;

        //Set Spell Max Uses Per Game
        newSpell.GetComponent<SpellScript>().SpellMaxUsesPerGame = -1;

        //Set Spell Type
        newSpell.GetComponent<SpellScript>().SpellType = "Physical";
    }
}