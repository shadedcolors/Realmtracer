using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Tooltip
    [SerializeField] private TooltipScript tooltip;

    //Progress Bars
    [SerializeField] private Image healthImage;
    [SerializeField] public Image manaSphere;
    [SerializeField] private Image spellQueueTimer;

    //Prefabs
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spellPrefab;

    //Panels
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] public GameObject spellInventoryPanel;
    [SerializeField] private GameObject spellQueuePanel;
    [SerializeField] public GameObject statusEffectPanel;

    //All Spell Icons
    [SerializeField] private Sprite[] allSpellImages;

    //Toggle Combat
    public bool inCombat = false;

    //List of Enemies
    public List<GameObject> listOfEnemies = new List<GameObject>();

    //-------SPELL QUEUE STUFF-------
    //Spell Queue Length
    private int spellQueueLength = 5;

    //Spell Queue Timer
    private float spellQueueTimeAmount = 5.0f;

    //Empty Queue check
    private bool isQueueEmpty = true;

    //-------Player Stats-------
    private float maxHealth = 15;
    private float curHealth = 15;
    private float maxMana = 15;
    private float curMana = 15;

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

    public bool IsQueueEmpty
    {
        get { return isQueueEmpty; }
        set { isQueueEmpty = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public float CurHealth
    {
        get { return curHealth; }
        set { curHealth = value;
            if (curHealth < 0) { curHealth = 0; }
            if (curHealth > MaxHealth) { curHealth = MaxHealth; }
        }
    }

    public float MaxMana
    {
        get { return maxMana; }
        set { maxMana = value; }
    }

    public float CurMana
    {
        get { return curMana; }
        set
        {
            curMana = value;
            if (curMana < 0) { curMana = 0; }
            if (curMana > maxMana) { curMana = maxMana; }
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check if in Combat
        if (inCombat)
        {
            healthImage.fillAmount = CurHealth / MaxHealth;
            Debug.Log(healthImage.fillAmount);

            //-------Do spell queue timer stuff-------
            //If the queue is not empty...
            if (!IsQueueEmpty)
            {
                //Count down
                spellQueueTimer.fillAmount -= 1.0f / SpellQueueTimeAmount * Time.deltaTime;

                //If queue timer is over...
                if (spellQueueTimer.fillAmount <= 0)
                {
                    //Check if the spell queue is empty
                    if (spellQueuePanel.transform.childCount <= 1)
                    {
                        IsQueueEmpty = true;
                    }

                    //Activate First Spell
                    spellQueuePanel.transform.GetChild(0).gameObject.GetComponent<QueueSpellScript>().spell.GetComponent<SpellScript>().ActivateSpell();

                    //Unparent Queue Spell before destroying
                    //spellQueuePanel.transform.GetChild(0).parent = null;

                    //Delete Queue Spell from Queue
                    Destroy(spellQueuePanel.transform.GetChild(0).gameObject);

                    //Reset spell queue progress bar
                    spellQueueTimer.fillAmount = 1;

                    Debug.Log(spellQueuePanel.transform.childCount);
                }
            }
        }

        //Toggle Combat
        if (Input.GetKeyDown(KeyCode.S))
        {
            inCombat = !inCombat;
        }

        //-------Player Spell Creation Test-------
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CreateSpell(spellInventoryPanel, gameObject, listOfEnemies[0]);
        }

        //-------Enemy Creation Test-------
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnemyScript newEnemy = Instantiate(enemyPrefab, enemyPanel.transform).GetComponent<EnemyScript>();

            newEnemy.SpellCastInterval = 1f;
            newEnemy.SpellCastPreCooldown = 2f;
            newEnemy.MaxHealth = 10f;

            listOfEnemies.Add(newEnemy.gameObject);
        }
    }
    
    //Create Spell Test
    public void CreateSpell(GameObject panel, GameObject owner, GameObject target)
    {
        //Create New Spell
        SpellScript newSpell = Instantiate(spellPrefab, panel.transform).GetComponent<SpellScript>();

        //Set Spell Icon
        newSpell.transform.Find("Spell Sprite").GetComponent<Image>().sprite = allSpellImages[Random.Range(0, allSpellImages.Length)]; //Change this to a set value later!

        newSpell.spellName = "Slash";

        //Set Spell Owner
        newSpell.SpellOwner = owner;

        //Set Spell Target
        newSpell.SpellTarget = target;
            
        //Set Spell Effect - Damage
        newSpell.DealsDamage = true;
        newSpell.DamageAmount = 10;

        newSpell.RegenerateEffect = true;
        newSpell.RegenerateAmountPerSecond = 1f;
        newSpell.RegenerateTotalTime = 4f;

        //Set Spell Mana Cost
        newSpell.SpellManaCost = 1;

        //Set Spell Cooldown
        newSpell.SpellCooldown = 4;

        //Set Spell Max Uses Per Combat
        newSpell.SpellMaxUsesPerCombat = -1;

        //Set Spell Max Uses Per Game
        newSpell.SpellMaxUsesPerGame = -1;

        //Set Spell Type
        newSpell.SpellType = "Physical";
    }
}