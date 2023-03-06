using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QueueSpellScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject spell;
    private SpellScript spellScript;

    private void Start()
    {
        spellScript = spell.GetComponent<SpellScript>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        spellScript.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        spellScript.OnPointerExit(eventData);
    }
}
