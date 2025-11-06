using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class enemyManager : MonoBehaviour
{
   public int armyNo, initialAmount;
   public SpriteRenderer conquerTerritoryColor;
   public bool underAttack;
   protected Coroutine refectionCoroutine; // Coroutine referansı
   protected TextMeshPro labelReference; // Text referansı

   protected virtual void Awake()
   {
      InitializeArmyCount();
   }

   private void InitializeArmyCount()
   {
      // Inspector'da manuel değer girilmişse onu kullan
      // Sadece armyNo 0 ise random sayı ata
      if (armyNo == 0)
      {
         armyNo = Random.Range(10, 25);
      }

      // Her durumda initialAmount'u ayarla
      initialAmount = armyNo;
   }

   protected void ConquerTerritory(TextMeshPro labelNo)
   {
      armyNo--;
      labelNo.text = armyNo.ToString();
      labelReference = labelNo; // Referansı sakla

      // Bölge fethedildi
      if (armyNo == 0)
      {
         conquerTerritoryColor.color = PlayerManager.playerManagerInstance.playerColor;

         // Yenileme coroutine'i durdur
         if (refectionCoroutine != null)
         {
            StopCoroutine(refectionCoroutine);
            refectionCoroutine = null;
         }
      }
      else
      {
         // Saldırı altında, yenilemeyi durdur
         underAttack = true;
         if (refectionCoroutine != null)
         {
            StopCoroutine(refectionCoroutine);
            refectionCoroutine = null;
         }
      }

      // Oyuncu askeri kalmadıysa ve bu bölgenin askeri varsa, yenilemeye başla
      if (PlayerManager.playerManagerInstance.playerArmyNo == 0 && armyNo > 0)
      {
         underAttack = false;
         if (refectionCoroutine == null)
         {
            refectionCoroutine = StartCoroutine(RefectionForces());
         }
      }
   }

   protected IEnumerator RefectionForces()
   {
      // Başlamadan önce kısa bir bekleme
      yield return new WaitForSeconds(1f);

      while (armyNo < initialAmount)
      {
         armyNo++;

         // Text'i güncelle
         if (labelReference != null)
         {
            labelReference.text = armyNo.ToString();
         }

         yield return new WaitForSeconds(0.5f);
      }

      refectionCoroutine = null; // Tamamlandı
   }

}


