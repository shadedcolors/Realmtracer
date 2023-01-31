using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    //Get Game Manager
    private GameManager gameManager;

    //Prefabs
    [SerializeField] private GameObject spellPrefab;

    //Panels
    [SerializeField] private GameObject enemySpellInventoryPanel;

    //Enemy Health Bar
    private Image enemyHealthBar;

    //Enemy Name Text
    private TMP_Text enemyNameText;

    //Enemy Sprite
    private Image currentEnemyImage;

    //Enemy Sprites
    [SerializeField] private Sprite[] allEnemyImages;

    //-------Enemy Stats-------

    //-------Properties-------

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
            gameManager.CreateSpell(enemySpellInventoryPanel);
        }
    }
}
