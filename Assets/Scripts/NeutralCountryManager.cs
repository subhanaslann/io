using System.Collections;
using TMPro;
using UnityEngine;

public class NeutralCountryManager : enemyManager
{
    [SerializeField] private TextMeshPro forceLabel;

    [Header("Neutral Settings")]
    public bool isNeutral = true;
    public Color neutralColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Gri renk
    private Color ownerColor;
    private Coroutine neutralRefectionCoroutine;

    protected override void Awake()
    {
        // Base class initialization (armyNo ayarlama)
        base.Awake();
        // NeutralCountryManager'a özel initialization varsa buraya
    }

    private void Start()
    {
        forceLabel = GetComponentInChildren<TextMeshPro>();
        forceLabel.text = armyNo.ToString();
        conquerTerritoryColor = transform.parent.GetComponent<SpriteRenderer>();
        labelReference = forceLabel; // Base class'a referansı ver

        // Başlangıçta nötr renk
        if (isNeutral)
        {
            conquerTerritoryColor.color = neutralColor;
            // NOT: Oyun başında yenileme BAŞLATILMIYOR
            // Sadece saldırı sonrası asker azaldıysa yenileme başlayacak
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncu askerlerinden darbe aldığında
        if (other.CompareTag("Player"))
        {
            // Yenileme coroutine'lerini durdur
            StopRefectionSystems();

            ConquerTerritory(forceLabel);

            // Eğer ele geçirildiyse (armyNo = 0) ve nötr idiyse
            if (armyNo == 0 && isNeutral)
            {
                isNeutral = false;
                ownerColor = PlayerManager.playerManagerInstance.playerColor;
            }
            else if (armyNo > 0 && isNeutral && armyNo < initialAmount)
            {
                // Hala nötr ve askeri var ama maksimumun altında, yenilemeye başla
                if (neutralRefectionCoroutine == null)
                {
                    neutralRefectionCoroutine = StartCoroutine(NeutralRefection());
                }
            }
        }

        // AI askerlerinden darbe aldığında
        if (other.CompareTag("AIEnemy"))
        {
            // Yenileme coroutine'lerini durdur
            StopRefectionSystems();

            ConquerTerritoryByAI(forceLabel);

            // Hala nötr ve askeri var ama maksimumun altında, yenilemeye başla
            if (armyNo > 0 && isNeutral && armyNo < initialAmount && neutralRefectionCoroutine == null)
            {
                neutralRefectionCoroutine = StartCoroutine(NeutralRefection());
            }
        }
    }

    // Yenileme sistemlerini durdur
    private void StopRefectionSystems()
    {
        if (neutralRefectionCoroutine != null)
        {
            StopCoroutine(neutralRefectionCoroutine);
            neutralRefectionCoroutine = null;
        }

        if (refectionCoroutine != null)
        {
            StopCoroutine(refectionCoroutine);
            refectionCoroutine = null;
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

    // Nötr bölgeler için yavaş asker yenileme
    IEnumerator NeutralRefection()
    {
        yield return new WaitForSeconds(2f); // Saldırıdan sonra bekleme

        while (isNeutral && armyNo < initialAmount)
        {
            armyNo++;

            if (forceLabel != null)
            {
                forceLabel.text = armyNo.ToString();
            }

            yield return new WaitForSeconds(1f); // Nötrler daha yavaş yenilenir
        }

        neutralRefectionCoroutine = null;
    }
}
