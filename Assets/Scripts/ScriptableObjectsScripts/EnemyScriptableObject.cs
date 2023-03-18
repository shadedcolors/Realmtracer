using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public string enemyName = "";

    public Sprite enemySprite = null;

    public float enemySpellCastInterval = 0f;
    public float enemySpellCastPreCooldown = 0f;
    public float enemyMaxHealth = 0f;

    public SpellScriptableObject[] enemySpells;
}