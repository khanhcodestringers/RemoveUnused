using UnityEngine;
using System.Collections;
using DG.Tweening;
namespace Mio.TileMaster {
    public class NoteBonus : NoteSimple {
        public BonusType bonusType;
        public Transform objMove;
        public override void Press (TouchCover _touchCover) {
            //this.touchCover = _touchCover;
            if (!isClickable) {
                return;
            }
            isClickable = false;
            isFinish = true;
            InGameUIController.Instance.gameplay.ProcessBonusTile(this);
            //Debug.Log("goeffect");
            DiamondEffectController.Instance.PlayEffect(this.transform.position);
            EffectWhenFinish();
			// AchievementHelper.Instance.LogAchievement("pickDiamondInGame");
        }
    }
}