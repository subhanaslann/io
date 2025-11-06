using TMPro;
using UnityEngine;

public class CountryManager : enemyManager
{
   [SerializeField] private TextMeshPro forceLabel;

   private void Start()
   {
      forceLabel = GetComponentInChildren<TextMeshPro>();
      forceLabel.text = armyNo.ToString();
      conquerTerritoryColor = transform.parent.GetComponent<SpriteRenderer>();
      labelReference = forceLabel; // Base class'a referansı ver
   }

   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         ConquerTerritory(forceLabel);
      }
   }
}
