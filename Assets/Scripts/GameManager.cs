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
    [SerializeField] public GameObject spellInventoryPanel;
    [SerializeField] private GameObject spellQueuePanel;

    //All Spell Icons
    [SerializeField] private Sprite[] allSpellImages;

    //Toggle Combat
    public bool inCombat = false;

    //-------SPELL QUEUE STUFF-------
    //Spell Queue Length
    private int spellQueueLength = 5;

    //Spell Queue Timer
    private float spellQueueTimeAmount = 10.0f;

    //Empty Queue check
    private bool isQueueEmpty = true;

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

    public bool IsQueueEmpty
    {
        get { return isQueueEmpty; }
        set { isQueueEmpty = value; }
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
            //-------Progress Bar Tests-------
            healthSphere.fillAmount -= 0.00024f;
            manaSphere.fillAmount -= 0.0001f;

            //-------Do spell queue timer stuff-------
            //If the queue is not empty...
            if (!IsQueueEmpty)
            {
                //Count down
                spellQueueTimer.fillAmount -= 1.0f / spellQueueTimeAmount * Time.deltaTime;

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
            CreateSpell(spellInventoryPanel, gameObject);
        }

        //-------Enemy Creation Test-------
        if (Input.GetKeyDown(KeyCode.E))
        {
            var newEnemy = Instantiate(enemyPrefab, enemyPanel.transform);

            newEnemy.GetComponent<EnemyScript>().SpellCastInterval = 3f;
            newEnemy.GetComponent<EnemyScript>().SpellCastPreCooldown = 1f;
        }
    }
    
    //Create Spell Test
    public void CreateSpell(GameObject panel, GameObject owner)
    {
        //Create New Spell
        var newSpell = Instantiate(spellPrefab, panel.transform);

        //Set Spell Icon
        newSpell.transform.Find("Spell Sprite").GetComponent<Image>().sprite = allSpellImages[Random.Range(0, allSpellImages.Length)]; //Change this to a set value later!

        //Set Spell Owner
        newSpell.GetComponent<SpellScript>().SpellOwner = owner;

        //Set Spell Target
        newSpell.GetComponent<SpellScript>().SpellTarget = null; //Add this later!

        //Set Spell Effect - Damage
        newSpell.GetComponent<SpellScript>().DealsDamage = true;
        newSpell.GetComponent<SpellScript>().DamageAmount = 1;

        //Set Spell Mana Cost
        newSpell.GetComponent<SpellScript>().SpellManaCost = 1;

        //Set Spell Cooldown
        newSpell.GetComponent<SpellScript>().SpellCooldown = 4;

        //Set Spell Max Uses Per Combat
        newSpell.GetComponent<SpellScript>().SpellMaxUsesPerCombat = -1;

        //Set Spell Max Uses Per Game
        newSpell.GetComponent<SpellScript>().SpellMaxUsesPerGame = -1;

        //Set Spell Type
        newSpell.GetComponent<SpellScript>().SpellType = "Physical";
    }
}