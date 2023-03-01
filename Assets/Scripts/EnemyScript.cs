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

    //Enemy Health Bar
    private Image enemyHealthBar;

    //Enemy Name Text
    private TMP_Text enemyNameText;

    //Enemy Sprite
    private Image currentEnemyImage;

    //Enemy Sprites
    [SerializeField] private Sprite[] allEnemyImages;

    //Next Spell Chosen
    private GameObject nextSpellToUse;

    //-------Enemy Stats-------

    //Spell Cast Interval
    private float spellCastInterval;
    private float spellCastIntervalTimer;

    //Spell Cast PreCooldown
    private float spellCastPreCooldown;
    private float spellCastPreCooldownTimer;

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

    // Start is called before the first frame update
    void Start()
    {
        //Get Game Manager
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Get Parts of the Enemy Prefab
        enemyHealthBar = transform.Find("Enemy Health Bar").GetComponent<Image>();
        enemyNameText = transform.Find("Enemy Name").GetComponent<TMP_Text>();
        currentEnemyImage = transform.Find("Enemy Sprite").GetComponent<Image>();

        //Set the Enemy's Sprite and Name Randomly
        currentEnemyImage.sprite = allEnemyImages[Random.Range(0, allEnemyImages.Length)];
        enemyNameText.text = currentEnemyImage.sprite.name;

        //Set Spell Cast Interval
        SpellCastIntervalTimer = SpellCastInterval;

        //Set Spell Cast PreCooldown
        SpellCastPreCooldownTimer = SpellCastPreCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        //Enemy Health Test
        enemyHealthBar.fillAmount -= 0.00024f;

        //Enemy Spell Creation Test
        //-------Player Spell Creation Test-------
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.CreateSpell(enemySpellInventoryPanel, gameObject);
        }

        //Choose Next Spell to use
        if (gameManager.inCombat)
        {
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
            nextSpellToUse = enemySpellInventoryPanel.transform.GetChild(randomSpell).gameObject;
        }
    }

    public void AddSpellToQueue()
    {
        //Check for cooldown before adding the spell to the queue
        if (nextSpellToUse.GetComponent<SpellScript>().spellCooldownAmount <= 0)
        {
            if (nextSpellToUse.GetComponent<SpellScript>().AddSpellToQueue())
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

    public void DoSpellCastInterval()
    {
        //If not on cooldown...
        if (nextSpellToUse.GetComponent<SpellScript>().SpellCooldownAmount <= 0)
        {
            //Count down Spell Cast Interval
            if (SpellCastIntervalTimer > 0)
            {
                SpellCastIntervalTimer -= Time.deltaTime;
                Debug.Log("Interval: " + SpellCastIntervalTimer);
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
            SpellCastPreCooldownTimer -= Time.deltaTime;
            Debug.Log("Cooldown: " + SpellCastPreCooldownTimer);
        }
        else
        {
            AddSpellToQueue(); //Draw red or green to determine success

            SpellCastIntervalTimer = SpellCastInterval;
            SpellCastPreCooldownTimer = SpellCastPreCooldown;
        }
    }
}
