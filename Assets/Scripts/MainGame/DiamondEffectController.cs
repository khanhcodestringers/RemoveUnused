using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Mio.TileMaster
{
    public class DiamondEffectController : MonoSingleton<DiamondEffectController>
    {
       public Transform[] listDiamondIMG;
       public Transform tagetTranform;
       public Animation anmTop;
        public void PlayEffect(Vector3 diamondPos)
        {
            for (int i = 0; i < listDiamondIMG.Length; i++) 
            {
                if(!listDiamondIMG[i].gameObject.activeSelf)
                {
                    listDiamondIMG[i].gameObject.SetActive(true);
                    anmTop.Play();
                    DOTween.Sequence().Append(listDiamondIMG[i].DOMove(tagetTranform.position,.8f)).Play().OnComplete(() => 
                    { 

                        listDiamondIMG[i].gameObject.SetActive(false);
                        listDiamondIMG[i].localPosition = Vector3.one;

                    });
                    break;
                }
            }
        }
    }
}
