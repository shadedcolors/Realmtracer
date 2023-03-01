using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellScript : MonoBehaviour
{
    //Get Game Manager
    private GameManager gameManager;

    //Get Spell Queue Panel
    private GameObject spellQueuePanel;

    //Get Queue Spell Prefab
    [SerializeField] private GameObject queueSpellPrefab;

    //Get Spell Cooldown Image
    [SerializeField] public Image spellCooldownImage;

    //Get Spell Cooldown Text
    [SerializeField] public TextMeshProUGUI spellCooldownText;

    //Spell Owner
    private GameObject spellOwner;

    //Spell Target
    private GameObject spellTarget; //make this a list when you need multiple enemies at once

    //Spell Owner Sprites
    [SerializeField] private Sprite playerOwnedSprite;
    [SerializeField] private Sprite enemyOwnedSprite;

    [SerializeField] public Image nextSpellOutline;

    //-------Spell Effects-------
    private bool dealsDamage;
    private float damageAmount;

    //-------Spell Stats-------
    private float spellManaCost;
    private float spellCooldown;
    public float spellCooldownAmount;
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

    public float SpellCooldownAmount
    {
        get { return spellCooldownAmount; }
        set { spellCooldownAmount = value; }
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

    private void Awake()
    {
        //Set Cooldown info
        spellCooldownImage.enabled = false;
        spellCooldownText.enabled = false;

        //Disable Next Spell Outline
        nextSpellOutline.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Get Game Manager
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Get Spell Queue Panel
        spellQueuePanel = GameObject.Find("Spell Queue Panel");
    }

    private void Update()
    {
        //Do Cooldown
        if (spellCooldownAmount > 0)
        {
            spellCooldownAmount -= Time.deltaTime;
        }
        else
        {
            spellCooldownImage.enabled = false;
            spellCooldownText.enabled = false;
        }

        spellCooldownImage.fillAmount = spellCooldownAmount / spellCooldown;
        spellCooldownText.text = Mathf.Round(spellCooldownAmount).ToString();

    }

    public void ButtonAddSpellToQueue()
    {
        AddSpellToQueue();
    }

    //Put Spell On Queue
    public bool AddSpellToQueue()
    {
        //Check if player is on cooldown
        if (!spellCooldownImage.enabled)
        {
            //Check for open slot in spell queue
            if (spellQueuePanel.transform.childCount < gameManager.SpellQueueLength)
            {
                //Create queue spell
                var newQueueSpell = Instantiate(queueSpellPrefab, spellQueuePanel.transform);

                //Add Icon to queue spell
                newQueueSpell.GetComponent<Image>().sprite = transform.Find("Spell Sprite").GetComponent<Image>().sprite;

                //Connect Queue Spell to Spell
                newQueueSpell.GetComponent<QueueSpellScript>().spell = gameObject;

                //color the queue spell depending on the owner
                if (SpellOwner == gameManager.gameObject)
                {
                    newQueueSpell.transform.GetChild(0).GetComponent<Image>().sprite = playerOwnedSprite;
                }
                else
                {
                    newQueueSpell.transform.GetChild(0).GetComponent<Image>().sprite = enemyOwnedSprite;
                }

                gameManager.IsQueueEmpty = false;

                //Set cooldown for player
                if (spellOwner == gameManager.gameObject)
                {
                    for (int i = 0; i < gameManager.spellInventoryPanel.transform.childCount; i++)
                    {
                        gameManager.spellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().spellCooldownImage.enabled = true;
                        gameManager.spellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().spellCooldownText.enabled = true;
                        gameManager.spellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().spellCooldownAmount = spellCooldown;
                    } 
                }
                //Set cooldown for enemy
                else
                {
                    for (int i = 0; i < spellOwner.GetComponent<EnemyScript>().enemySpellInventoryPanel.transform.childCount; i++)
                    {
                        spellOwner.GetComponent<EnemyScript>().enemySpellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().spellCooldownImage.enabled = true;
                        spellOwner.GetComponent<EnemyScript>().enemySpellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().spellCooldownText.enabled = true;
                        spellOwner.GetComponent<EnemyScript>().enemySpellInventoryPanel.transform.GetChild(i).GetComponent<SpellScript>().spellCooldownAmount = spellCooldown;
                    }
                }

                return true;
            }  
            else
            {
                Debug.Log("No Space in Queue");
                return false;
            }
        }
        return false;
    }

    //Activate the spell's effect (when first in the queue)
    public void ActivateSpell()
    {
        Debug.Log("Activate!");
    }
}