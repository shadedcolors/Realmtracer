using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectScript : MonoBehaviour
{
    //Owner Panel
    public GameObject ownerPanel;

    //Owner
    public GameObject ownerObject;

    //Owner Stats
    private Image ownerHealthFillAmount;
    private float ownerMaxHealth;

    //ICONS
    [SerializeField] private Sprite regenerateIcon;
    [SerializeField] private Sprite otherIcon;


    //-------EFFECTS-------
    //Regenerate
    private bool regenerateEffect;
    private float regenerateAmountPerSecond;
    private float regenerateTotalTime;

    private float regenerateTimer;

    //Properties
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

    private void Start()
    {
        //Get Owner Health
        if (ownerObject == GameObject.Find("Game Manager").gameObject)
        {
            ownerHealthFillAmount = ownerObject.GetComponent<GameManager>().healthSphere;
            ownerMaxHealth = ownerObject.GetComponent<GameManager>().PlayerHealth;
        }
        else
        {
            ownerHealthFillAmount = ownerObject.GetComponent<EnemyScript>().enemyHealthBar;
            ownerMaxHealth = ownerObject.GetComponent<EnemyScript>().EnemyHealth;
        }

        //SET ICON
        //Regenerate
        if (RegenerateEffect)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = regenerateIcon;
        }
    }

    private void Update()
    {
        //Regenerate
        if (RegenerateEffect)
        {
            if (regenerateTimer < RegenerateTotalTime)
            {
                regenerateTimer += Time.deltaTime;

                ownerHealthFillAmount.fillAmount += (RegenerateAmountPerSecond * Time.deltaTime) / ownerMaxHealth;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
