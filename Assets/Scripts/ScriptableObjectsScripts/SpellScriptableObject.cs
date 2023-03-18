using UnityEngine;

[CreateAssetMenu(fileName = "SpellScriptableObject", menuName = "ScriptableObjects/Spell")]
public class SpellScriptableObject : ScriptableObject
{
    public string spellName = "";
    public Sprite spellIcon = null;

    public bool dealsDamage = false;
    public int damageAmount = 0;

    public bool regenerateEffect = false;
    public float regenerateAmountPerSecond = 0f;
    public float regenerateTotalTime = 0f;

    public int spellManaCost = 0;
    
    public int spellCooldown = 0;

    public int spellMaxUsesPerCombat = 0;

    public int spellMaxUsesPerGame = 0;

    public string spellType = "";
}