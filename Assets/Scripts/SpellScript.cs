using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SpellScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Get Game Manager
    private GameManager gameManager;

    //Get Spell Queue Panel
    private GameObject spellQueuePanel;

    [SerializeField] private GameObject statusEffectPrefab;

    //Get Queue Spell Prefab
    [SerializeField] private GameObject queueSpellPrefab;

    //Get Spell Cooldown Image
    [SerializeField] public Image spellCooldownImage;

    //Get Spell Cooldown Text
    [SerializeField] public TextMeshProUGUI spellCooldownText;

    //Get Spell PreCooldown Text
    [SerializeField] public TextMeshProUGUI spellPreCooldownText;

    [SerializeField] public Image spellPreCooldownShade;

    //Spell Owner
    private GameObject spellOwner;

    //Spell Target
    private GameObject spellTarget; //make this a list when you need multiple enemies at once

    //Spell Owner Sprites
    [SerializeField] private Sprite playerOwnedSprite;
    [SerializeField] private Sprite enemyOwnedSprite;

    //Next Spell To Use Outline
    [SerializeField] public Image nextSpellOutline;

    //Spell Name
    public string spellName;

    //-------Spell Effects-------
    private bool dealsDamage;
    private float damageAmount;

    private bool regenerateEffect;
    private float regenerateAmountPerSecond;
    private float regenerateTotalTime;

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

    public bool RegenerateEffect
    {
        get { return regenerateEffect; }
        set { regenerateEffect = value; }
    }

    public float RegenerateAmountPerSecond
    {
        get { return regenerateAmountPerSecond; }
        set { regenerateAmountPerSecond = value; }
    }

    public float RegenerateTotalTime
    {
        get { return regenerateTotalTime; }
        set { regenerateTotalTime = value; }
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
        //Set Cooldown Info
        spellCooldownImage.enabled = false;
        spellCooldownText.enabled = false;

        //Set PreCooldown Info
        spellPreCooldownText.enabled = false;

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
        DoCooldown();
    }

    public void DoCooldown()
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

    //Player Spell (Click On The Spell Button)
    public void ButtonAddSpellToQueue()
    {
        //Only allow clicking on a spell if the owner is the player
        if (spellOwner == gameManager.gameObject)
        {
            AddSpellToQueue();
        }
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
        //If Spell deals damage...
        if (DealsDamage == true)
        {
            //If the spell target is an enemy...
            if (spellTarget != gameManager.gameObject)
            {
                //Damage the target enemy
                spellTarget.GetComponent<EnemyScript>().CurHealth -= DamageAmount;
            }
            else
            {
                //Otherwise damage the player
                spellTarget.GetComponent<GameManager>().CurHealth -= DamageAmount;
            }
        }

        AddStatusEffects();
    }

    public void AddStatusEffects()
    {
        //Get the correct Status Effect Panel
        GameObject ownerPanel = GetStatusEffectPanel();

        //Regenerate
        if (RegenerateEffect)
        {
            var statusEffect = Instantiate(statusEffectPrefab, ownerPanel.transform).GetComponent<StatusEffectScript>();

            //statusEffect.ownerPanel = ownerPanel;
            statusEffect.statusEffectOwner = SpellOwner;

            statusEffect.RegenerateEffect = RegenerateEffect;
            statusEffect.RegenerateAmountPerSecond = RegenerateAmountPerSecond;
            statusEffect.RegenerateTotalTime = RegenerateTotalTime;
        }
    }

    public GameObject GetStatusEffectPanel()
    {
        //Get spell owner
        if (SpellOwner == gameManager.gameObject)
        {
            return SpellOwner.GetComponent<GameManager>().statusEffectPanel;
        }
        else
        {
            return SpellOwner.GetComponent<EnemyScript>().enemyStatusEffectPanel;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Make Delegate for Tooltip
        System.Func<string> getTooltipTextFunc = () =>
        {
            string tooltipText = "";

            //Add Spell Name
            tooltipText += "<color=#00ff00>" + spellName + "</color>\n";

            //Add Damage
            if (DealsDamage)
            {
                tooltipText += "Damage: " + DamageAmount + "\n";
            }

            //Add Regenerate
            if (RegenerateEffect)
            {
                tooltipText += "Adds Regenerate: " + RegenerateAmountPerSecond + "DPS for " + RegenerateTotalTime + "seconds\n";
            }

            //Add Mana Cost
            tooltipText += "Mana Cost: " + SpellManaCost + "\n";

            //Add Cooldown
            tooltipText += "Cooldown: " + SpellCooldown + "\n";

            //Add Spell Type
            tooltipText += "Type: " + SpellType + "\n";

            //Add Max Uses Per Combat
            tooltipText += "Max Uses Per Combat: " + SpellMaxUsesPerCombat + "\n";

            //Add Max Uses Per Game
            tooltipText += "Max Uses Per Game: " + SpellMaxUsesPerGame + "\n";

            return tooltipText;
        };
        TooltipScript.ShowTooltip_Static(getTooltipTextFunc);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipScript.HideTooltip_Static();
    }
}