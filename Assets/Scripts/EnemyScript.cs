using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    //Get Game Manager
    private GameManager gameManager;

    //Prefabs
    [SerializeField] private GameObject spellPrefab;

    //Panels
    [SerializeField] public GameObject enemySpellInventoryPanel;
    [SerializeField] public GameObject enemyStatusEffectPanel;

    //Enemy Health Bar
    private Image healthImage;

    //Enemy Name Text
    private TMP_Text enemyNameText;

    //Enemy Sprite
    private Image currentEnemyImage;

    //Enemy Sprites
    [SerializeField] private Sprite[] allEnemyImages;

    //Next Spell Chosen
    private SpellScript nextSpellToUse;

    //-------Enemy Stats-------

    //Spell Cast Interval
    private float spellCastInterval;
    private float spellCastIntervalTimer;

    //Spell Cast PreCooldown
    private float spellCastPreCooldown;
    private float spellCastPreCooldownTimer;

    //Health
    private float maxHealth;
    private float curHealth;

    //-------Properties-------
    public float SpellCastInterval
    {
        get { return spellCastInterval; }
        set { spellCastInterval = value; }
    }

    public float SpellCastIntervalTimer
    {
        get { return spellCastIntervalTimer; }
        set { spellCastIntervalTimer = value; }
    }

    public float SpellCastPreCooldown
    {
        get { return spellCastPreCooldown; }
        set { spellCastPreCooldown = value; }
    }

    public float SpellCastPreCooldownTimer
    {
        get { return spellCastPreCooldownTimer; }
        set { spellCastPreCooldownTimer = value; }
    }

    public float CurHealth
    {
        get { return curHealth; }
        set
        {
            curHealth = value;
            if (curHealth < 0) { curHealth = 0; }
            if (curHealth > MaxHealth) { curHealth = MaxHealth; }
        }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get Game Manager
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Get Parts of the Enemy Prefab
        healthImage = transform.Find("Enemy Health Bar").GetComponent<Image>();
        enemyNameText = transform.Find("Enemy Name").GetComponent<TMP_Text>();
        currentEnemyImage = transform.Find("Enemy Sprite").GetComponent<Image>();

        //Set the Enemy's Sprite and Name Randomly
        currentEnemyImage.sprite = allEnemyImages[Random.Range(0, allEnemyImages.Length)];
        enemyNameText.text = currentEnemyImage.sprite.name;

        //Set Spell Cast Interval
        SpellCastIntervalTimer = SpellCastInterval;

        //Set Spell Cast PreCooldown
        SpellCastPreCooldownTimer = SpellCastPreCooldown;

        //Set Current Health
        CurHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Enemy Spell Creation Test
        //-------Player Spell Creation Test-------
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.CreateSpell(enemySpellInventoryPanel, gameObject, gameManager.gameObject);
        }

        //Choose Next Spell to use
        if (gameManager.inCombat)
        {
            healthImage.fillAmount = CurHealth / MaxHealth;

            ChooseNextSpellToUse();
            DoSpellCastInterval();
        }
    }

    public void ChooseNextSpellToUse()
    {
        int randomSpell = 0;

        //If a new spell hasn't been chosen...
        if (nextSpellToUse == null)
        {
            //Choose a new spell 
            randomSpell = Random.Range(0, enemySpellInventoryPanel.transform.childCount-1);

            //Show Next Spell Outline
            enemySpellInventoryPanel.transform.GetChild(randomSpell).GetComponent<SpellScript>().nextSpellOutline.enabled = true;

            //Save Next Spell to use
            nextSpellToUse = enemySpellInventoryPanel.transform.GetChild(randomSpell).gameObject.GetComponent<SpellScript>();
        }
    }

    public void DoSpellCastInterval()
    {
        //If not on cooldown...
        if (nextSpellToUse.SpellCooldownAmount <= 0)
        {
            //Count down Spell Cast Interval
            if (SpellCastIntervalTimer > 0)
            {
                SpellCastIntervalTimer -= Time.deltaTime;
            }
            else
            {
                DoSpellCastPreCooldown();
            }
        }
    }

    public void DoSpellCastPreCooldown()
    {
        if (SpellCastPreCooldownTimer > 0)
        {
            //Shade Spell


            //Show PreCooldown Number
            nextSpellToUse.spellPreCooldownText.enabled = true;
            nextSpellToUse.spellPreCooldownText.text = Mathf.Round(SpellCastPreCooldownTimer).ToString();

            //Count PreCooldown
            SpellCastPreCooldownTimer -= Time.deltaTime;
        }
        else
        {
            //Disable PreCooldown Number
            nextSpellToUse.spellPreCooldownText.enabled = false;

            //Add Spell to Queue When PreCooldown is done
            AddSpellToQueue(); //Draw red or green to determine success

            //Reset Spell Cast Interval and PreCooldown
            SpellCastIntervalTimer = SpellCastInterval;
            SpellCastPreCooldownTimer = SpellCastPreCooldown;
        }
    }

    public void AddSpellToQueue()
    {
        //Check for cooldown before adding the spell to the queue
        if (nextSpellToUse.spellCooldownAmount <= 0)
        {
            if (nextSpellToUse.AddSpellToQueue())
            {
                //Hide Next Spell Outline
                for (int i = 0; i < enemySpellInventoryPanel.transform.childCount; i++)
                {
                    enemySpellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().nextSpellOutline.enabled = false;
                }

                //Reset Next Spell to use
                nextSpellToUse = null;
            }
        }
    }
}
