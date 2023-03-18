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

    //Between Combat Stuff
    [SerializeField] private GameObject betweenCombatScreen;

    [SerializeField] private BetweenCombatOptionScript[] spellOptions;

    //All Spell Icons
    [SerializeField] private Sprite[] allSpellImages;

    //All Spells
    [SerializeField] public SpellScriptableObject[] allSpellScriptableObjects;

    //All Enemies
    [SerializeField] public EnemyScriptableObject[] allEnemyScriptableObjects;

    //Toggle Combat
    public bool inCombat = false;

    //List of Enemies
    public List<GameObject> listOfEnemies = new List<GameObject>(); //DELETE POSSIBLY????

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

        //Open Between Combat Screen
        if (Input.GetKeyDown(KeyCode.Return))
        {
            betweenCombatScreen.SetActive(!betweenCombatScreen.activeSelf);

            //Reset spell pool???

            //Set all 3 Spell Options
            for (int i = 0; i < 3; i++)
            {
                //Get a random spell from the spell pool
                int randomSpell = Random.Range(0, allSpellScriptableObjects.Length);

                SpellScriptableObject spellScriptableObject = allSpellScriptableObjects[randomSpell];

                spellOptions[i].SetSpellOption(spellScriptableObject, spellScriptableObject.spellName, spellScriptableObject.spellIcon);
            }
        }
    }

    //Create Enemy
    public GameObject CreateEnemy()
    {
        //Create Enemy Prefab
        EnemyScript newEnemy = Instantiate(enemyPrefab, enemyPanel.transform).GetComponent<EnemyScript>();

        //Get a random enemy scriptable object
        int randomEnemy = Random.Range(0, allEnemyScriptableObjects.Length);

        EnemyScriptableObject pickedEnemySO = allEnemyScriptableObjects[randomEnemy];

        //Set Enemy Name and Sprite
        newEnemy.currentEnemyImage.sprite = pickedEnemySO.enemySprite;
        newEnemy.enemyNameText.text = pickedEnemySO.enemyName;

        //Set Enemy Stats
        newEnemy.SpellCastInterval = pickedEnemySO.enemySpellCastInterval;
        newEnemy.SpellCastPreCooldown = pickedEnemySO.enemySpellCastPreCooldown;
        newEnemy.MaxHealth = pickedEnemySO.enemyMaxHealth;

        //Create Enemy Spells based on Enemy SO
        for (int i = 0; i < pickedEnemySO.enemySpells.Length; i++)
        {
            CreateSpell(newEnemy.enemySpellInventoryPanel, newEnemy.gameObject, gameObject, pickedEnemySO.enemySpells[i]);
        }

        //Add New Enemy to Enemy List
        listOfEnemies.Add(newEnemy.gameObject);

        //Return Enemy Object
        return newEnemy.gameObject;
    }
    
    //Create Spell
    public void CreateSpell(GameObject panel, GameObject owner, GameObject target, SpellScriptableObject spellSO)
    {
        //Create New Spell
        SpellScript newSpell = Instantiate(spellPrefab, panel.transform).GetComponent<SpellScript>();

        //Set Spell Icon
        newSpell.transform.Find("Spell Sprite").GetComponent<Image>().sprite = spellSO.spellIcon;

        newSpell.spellName = spellSO.spellName;

        //Set Spell Owner
        newSpell.SpellOwner = owner;

        //Set Spell Target
        newSpell.SpellTarget = target;

        //Set Spell Effect - Damage
        newSpell.DealsDamage = spellSO.dealsDamage;
        newSpell.DamageAmount = spellSO.damageAmount;

        newSpell.RegenerateEffect = spellSO.regenerateEffect;
        newSpell.RegenerateAmountPerSecond = spellSO.regenerateAmountPerSecond;
        newSpell.RegenerateTotalTime = spellSO.regenerateTotalTime;

        //Set Spell Mana Cost
        newSpell.SpellManaCost = spellSO.spellManaCost;

        //Set Spell Cooldown
        newSpell.SpellCooldown = spellSO.spellCooldown;

        //Set Spell Max Uses Per Combat
        newSpell.SpellMaxUsesPerCombat = spellSO.spellMaxUsesPerCombat;

        //Set Spell Max Uses Per Game
        newSpell.SpellMaxUsesPerGame = spellSO.spellMaxUsesPerGame;

        //Set Spell Type
        newSpell.SpellType = spellSO.spellType;
    }
}