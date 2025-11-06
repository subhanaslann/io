using TMPro;
using UnityEngine;

public class NeutralCountryManager : enemyManager
{
    [SerializeField] private TextMeshPro forceLabel;

    [Header("Neutral Settings")]
    public bool isNeutral = true;
    public Color neutralColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Gri renk
    private Color ownerColor;

    private void Start()
    {
        forceLabel = GetComponentInChildren<TextMeshPro>();
        forceLabel.text = armyNo.ToString();
        conquerTerritoryColor = transform.parent.GetComponent<SpriteRenderer>();

        // Başlangıçta nötr renk
        if (isNeutral)
        {
            conquerTerritoryColor.color = neutralColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncu askerlerinden darbe aldığında
        if (other.CompareTag("Player"))
        {
            ConquerTerritory(forceLabel);

            // Eğer ele geçirildiyse (armyNo = 0) ve nötr idiyse
            if (armyNo == 0 && isNeutral)
            {
                isNeutral = false;
                ownerColor = PlayerManager.playerManagerInstance.playerColor;
            }
        }

        // AI askerlerinden darbe aldığında
        if (other.CompareTag("AIEnemy"))
        {
            ConquerTerritoryByAI(forceLabel);
        }
    }

    // AI için özel fetih metodu
    private void ConquerTerritoryByAI(TextMeshPro labelNo)
    {
        armyNo--;
        labelNo.text = armyNo.ToString();

        if (armyNo == 0)
        {
            // AI'ya ait oldu
            if (AIEnemyManager.instance != null)
            {
                conquerTerritoryColor.color = AIEnemyManager.instance.enemyColor;
                isNeutral = false;
                ownerColor = AIEnemyManager.instance.enemyColor;
            }
        }
    }
}
