using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusEffectScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //Owner Panel
    //public GameObject ownerPanel;

    //Owner
    public GameObject statusEffectOwner;

    //Owner Stats
    private bool isPlayer;

    private GameManager playerScript;
    private EnemyScript enemyScript;

    //ICONS
    [SerializeField] private Sprite regenerateIcon;
    [SerializeField] private Sprite otherIcon;


    //-------EFFECTS-------
    public string statusEffectName;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Make Delegate for Tooltip
        System.Func<string> getTooltipTextFunc = () =>
        {
            string tooltipText = "";

            //Add Spell Name
            tooltipText += "<color=#ff0000>" + statusEffectName + "</color>\n";

            //Add Regenerate
            if (RegenerateEffect)
            {
                tooltipText += "Adds Regenerate: " + RegenerateAmountPerSecond + "DPS for " + RegenerateTotalTime + "seconds\n";
            }

            return tooltipText;
        };
        TooltipScript.ShowTooltip_Static(getTooltipTextFunc);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipScript.HideTooltip_Static();
    }

    private void Start()
    {
        //Get Scripts
        playerScript = statusEffectOwner.GetComponent<GameManager>();
        enemyScript = statusEffectOwner.GetComponent<EnemyScript>();

        //Get Owner
        if (statusEffectOwner == GameObject.Find("Game Manager").gameObject)
        {
            isPlayer = true;
        }
        else
        {
            isPlayer = false;
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
        //-------Regenerate-------
        if (RegenerateEffect)
        {
            //Regenerate Timer
            if (regenerateTimer < RegenerateTotalTime)
            {
                regenerateTimer += Time.deltaTime;

                //Adjust Owner's Health
                if (isPlayer)
                {
                    playerScript.CurHealth += (RegenerateAmountPerSecond * Time.deltaTime);
                }
                else
                {
                    enemyScript.CurHealth += (RegenerateAmountPerSecond * Time.deltaTime);
                }
            }
            else
            {
                //Remove Status Effect
                TooltipScript.HideTooltip_Static();
                Destroy(gameObject);
            }
        }
    }
}
