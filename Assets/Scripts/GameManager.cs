using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Tooltip
    [SerializeField] private TooltipScript tooltip;

    //Progress Bars
    [SerializeField] private Image healthImage;
    [SerializeField] public Image manaImage;
    [SerializeField] private Image spellQueueTimer;

    //Prefabs
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject spellPrefab;

    //Panels
    [SerializeField] private GameObject enemyPanel;
    [SerializeField] public GameObject spellInventoryPanel;
    [SerializeField] public GameObject spellQueuePanel;
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
    private float maxHealth = 15f;
    private float curHealth = 15f;
    private float maxMana = 15f;
    private float curMana = 15f;

    private float manaRegenerationPerSecond = 1f;

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

    public float ManaRegenerationPerSecond
    {
        get { return manaRegenerationPerSecond; }
        set { manaRegenerationPerSecond = value; }
    }

    // Update is called once per frame
    void Update()
    {
        //Check if in Combat
        if (inCombat)
        {
            //Set Health and Mana Sphere Images
            healthImage.fillAmount = CurHealth / MaxHealth;
            manaImage.fillAmount = CurMana / MaxMana;

            //Regenerate Mana Automatically
            if (CurMana < MaxMana)
            {
                CurMana += Time.deltaTime * ManaRegenerationPerSecond;
            }

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
                }
            }
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

    //Check for end of combat
    public void EndCombatCheck()
    {
        //check if all enemies are dead
        if (enemyPanel.transform.childCount <= 0)
        {
            //Open Between Combat Screen
            betweenCombatScreen.SetActive(true);

            //Stop Combat
            inCombat = false;
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
        newEnemy.SpellCastUsing = pickedEnemySO.enemySpellCastUsing;
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