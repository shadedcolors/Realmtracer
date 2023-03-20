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
    public TMP_Text enemyNameText;

    //Enemy Sprite
    public Image currentEnemyImage;

    //Enemy Sprites
    [SerializeField] private Sprite[] allEnemyImages;

    //Next Spell Chosen
    private SpellScript nextSpellToUse;

    [SerializeField] private TMP_Text enemyHealthBarText;

    //-------Enemy Stats-------

    //Spell Cast Interval
    private float spellCastInterval;
    private float spellCastIntervalTimer;

    //Spell Cast PreCooldown
    private float spellCastPreCooldown;
    private float spellCastPreCooldownTimer;

    //Spell Cast Using Timer
    private float spellCastUsing;
    private float spellCastUsingTimer;

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

    public float SpellCastUsing
    {
        get { return spellCastUsing; }
        set { spellCastUsing = value; }
    }

    public float SpellCastUsingTimer
    {
        get { return spellCastUsingTimer; }
        set { spellCastUsingTimer = value; }
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

        //Set Spell Cast Interval
        SpellCastIntervalTimer = SpellCastInterval * Random.Range(0.5f, 1.5f);

        //Set Spell Cast PreCooldown
        SpellCastPreCooldownTimer = SpellCastPreCooldown * Random.Range(0.5f, 1.5f);

        //Set Spell Cast Using
        SpellCastUsingTimer = SpellCastUsing * Random.Range(0.5f, 1.5f);

        //Set Current Health
        CurHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Choose Next Spell to use
        if (gameManager.inCombat)
        {
            //Draw Health Bar
            healthImage.fillAmount = CurHealth / MaxHealth;

            //Draw Health Bar Text
            enemyHealthBarText.text = Mathf.Round(CurHealth) + "/" + MaxHealth;

            //Destroy Enemy on Death
            if (CurHealth <= 0)
            {
                transform.SetParent(null);
                Destroy(gameObject);

                //Check for end of combat
                gameManager.EndCombatCheck();
            }

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
            randomSpell = Random.Range(0, enemySpellInventoryPanel.transform.childCount);

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
            TryAddSpellToQueue(); //Draw red or green to determine success
        }
    }

    public void TryAddSpellToQueue()
    {
        //Check for cooldown before adding the spell to the queue
        if (nextSpellToUse.spellCooldownAmount <= 0)
        {
            //Countdown spell Using Timer
            if (SpellCastUsingTimer > 0)
            {
                //Shade Spell
                nextSpellToUse.spellUsingShade.enabled = true;

                //Count Using Timer
                SpellCastUsingTimer -= Time.deltaTime;

                //Keep trying to put next spell on the queue
                if (nextSpellToUse.AddSpellToQueue())
                {
                    //Hide Next Spell Outline
                    for (int i = 0; i < enemySpellInventoryPanel.transform.childCount; i++)
                    {
                        enemySpellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().nextSpellOutline.enabled = false;
                    }

                    //Unshade Spell
                    nextSpellToUse.spellUsingShade.enabled = false;

                    //Reset Next Spell to use
                    nextSpellToUse = null;

                    //Reset all Timers when successfully putting next spell on the queue
                    SpellCastIntervalTimer = SpellCastInterval * Random.Range(0.5f, 1.5f);
                    SpellCastPreCooldownTimer = SpellCastPreCooldown * Random.Range(0.5f, 1.5f);
                    SpellCastUsingTimer = SpellCastUsing * Random.Range(0.5f, 1.5f);
                }
            }
            else
            {
                //Unshade Spell
                nextSpellToUse.spellUsingShade.enabled = false;

                //Reset all Timers when Spell Cast using Timer ends
                SpellCastIntervalTimer = SpellCastInterval * Random.Range(0.5f, 1.5f);
                SpellCastPreCooldownTimer = SpellCastPreCooldown * Random.Range(0.5f, 1.5f);
                SpellCastUsingTimer = SpellCastUsing * Random.Range(0.5f, 1.5f);
            }
        }
    }
}
