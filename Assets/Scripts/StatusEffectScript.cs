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
    private float ownerHealth;

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
        if (ownerObject == GameObject.Find("Game Manager"))
        {
            var ownerScript = ownerObject.GetComponent<GameManager>();
        }
        else
        {
            var ownerScript = ownerObject.GetComponent<EnemyScript>();
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

                //If the status effect is in the player's status effect panel...
                if (ownerObject == GameObject.Find("Game Manager"))
                {
                    ownerObject.GetComponent<GameManager>().PlayerHealth += RegenerateAmountPerSecond * Time.deltaTime;
                }
                else
                {
                    ownerObject.GetComponent<EnemyScript>().EnemyHealth += RegenerateAmountPerSecond * Time.deltaTime;
                }
                
            }
        }
    }
}
