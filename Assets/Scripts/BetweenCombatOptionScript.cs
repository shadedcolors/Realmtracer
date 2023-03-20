using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BetweenCombatOptionScript : MonoBehaviour
{
    //Get Game Manager
    [SerializeField] private GameManager gameManager;

    //Get Between Combat Screen
    [SerializeField] private GameObject betweenCombatScreen;

    //Get Player Spell Inventory
    [SerializeField] private GameObject spellInventoryPanel;

    //Get Spell Prefab
    [SerializeField] private GameObject spellPrefab;

    //Get Spell
    public SpellScriptableObject spellSO;

    //Get Option Objects
    [SerializeField] public TextMeshProUGUI spellOptionName;
    [SerializeField] public Image spellOptionIcon;

    public void SetSpellOption(SpellScriptableObject spellSO, string spellOptionName, Sprite spellOptionIcon)
    {
        this.spellSO = spellSO;
        this.spellOptionName.text = spellOptionName;
        this.spellOptionIcon.sprite = spellOptionIcon;
    }

    //Setup Combat
    public void StartCombat()
    {
        //Clear the spell Queue
        for (int i = 0; i < gameManager.spellQueuePanel.transform.childCount; i++)
        {
            Destroy(gameManager.spellQueuePanel.transform.GetChild(i).gameObject);
        }

        //Create the newly acquired player spell
        gameManager.CreateSpell(spellInventoryPanel, gameManager.gameObject, null, spellSO);
        //CreateItem();
        //CreatePassive();

        //Create an Enemy
        GameObject newEnemy = gameManager.CreateEnemy();

        //Set all player spells target to instantiated enemy
        for (int i = 0; i < spellInventoryPanel.transform.childCount; i++)
        {
            spellInventoryPanel.transform.GetChild(i).gameObject.GetComponent<SpellScript>().SpellTarget = newEnemy;
        }

        //Start the Battle
        gameManager.inCombat = true;

        //Close Between Combat screen
        betweenCombatScreen.SetActive(false);
    }
}