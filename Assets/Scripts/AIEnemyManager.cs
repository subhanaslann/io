using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AIEnemyManager : enemyManager
{
    public static AIEnemyManager instance;

    [Header("AI Settings")]
    public Color enemyColor = new Color(1f, 0.28f, 0.45f, 1f); // Kırmızı/pembe
    [SerializeField] private TextMeshPro forceLabel;
    [SerializeField] private float aiThinkInterval = 3f; // AI her 3 saniyede bir düşünür
    [SerializeField] private int minArmyToAttack = 15; // Saldırmak için minimum asker sayısı

    [Header("Attack Settings")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private Transform startPos;
    [SerializeField] private float[] angles = new float[] { -15f, 0f, 15f };
    [SerializeField] private float fireInterval = 0.2f;

    private List<NeutralCountryManager> neutralCountries = new List<NeutralCountryManager>();
    private Coroutine aiThinkingCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        forceLabel = GetComponentInChildren<TextMeshPro>();
        forceLabel.text = armyNo.ToString();
        conquerTerritoryColor = transform.parent.GetComponent<SpriteRenderer>();

        // AI rengini ayarla
        conquerTerritoryColor.color = enemyColor;

        // Tüm nötr bölgeleri bul
        FindNeutralCountries();

        // AI düşünmeye başla
        aiThinkingCoroutine = StartCoroutine(AIThinkingLoop());
    }

    private void FindNeutralCountries()
    {
        neutralCountries.Clear();
        NeutralCountryManager[] allNeutral = FindObjectsOfType<NeutralCountryManager>();
        neutralCountries.AddRange(allNeutral);
    }

    IEnumerator AIThinkingLoop()
    {
        // Oyun başında biraz bekle
        yield return new WaitForSeconds(5f);

        while (true)
        {
            // Yeterli asker varsa saldır
            if (armyNo >= minArmyToAttack)
            {
                Transform target = ChooseTarget();
                if (target != null)
                {
                    StartCoroutine(AttackTarget(target));
                }
            }

            // Bir sonraki düşünme zamanına kadar bekle
            yield return new WaitForSeconds(aiThinkInterval);
        }
    }

    // AI hedef seçme stratejisi
    private Transform ChooseTarget()
    {
        // Listeyi güncelle
        FindNeutralCountries();

        // En yakın nötr bölgeyi bul
        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (var neutral in neutralCountries)
        {
            if (neutral != null && neutral.isNeutral)
            {
                float distance = Vector3.Distance(transform.position, neutral.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = neutral.transform;
                }
            }
        }

        // Eğer nötr bölge yoksa, oyuncuya saldır
        if (closestTarget == null && PlayerManager.playerManagerInstance != null)
        {
            closestTarget = PlayerManager.playerManagerInstance.transform;
        }

        return closestTarget;
    }

    IEnumerator AttackTarget(Transform target)
    {
        int soldiersToSend = Mathf.Min(10, armyNo - 5); // Max 10 asker gönder, en az 5 bırak

        for (int i = 0; i < soldiersToSend; i++)
        {
            // 3 mermi gönder (oyuncu gibi)
            for (int j = 0; j < 3; j++)
            {
                if (bulletPrefab != null && startPos != null)
                {
                    var bullet = Instantiate(bulletPrefab, startPos.position, Quaternion.identity);
                    bullet.tag = "AIEnemy"; // AI mermisi olarak işaretle
                    bullet.Initialize(target.position, angles[j], target);
                }
            }

            armyNo--;
            forceLabel.text = armyNo.ToString();

            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncu saldırısı
        if (other.CompareTag("Player"))
        {
            ConquerTerritory(forceLabel);
        }
    }
}
