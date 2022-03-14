using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NumericSprite : MonoBehaviour {
    [Tooltip("Sprites to display text")]
    [SerializeField]
    private List<SpriteRenderer> spriteObjects;

    [Tooltip("Sprites to show in sprite objects")]
    [SerializeField]
    private List<Sprite> sprites;

    private static Dictionary<char, Sprite> spriteByName;
    private string currentText;
	public string text {
        get {
            return currentText;
        }

        set {
            currentText = value;
            RefreshUI();
        }
    }


    public Color color {
        get {
            if(spriteObjects != null) {
                return spriteObjects[0].color;
            }else {
                return Color.white;
            }
        }

        set {
            for(int i = 0; i < spriteObjects.Count; i++) {
                if (spriteObjects[i].gameObject.activeSelf) {
                    spriteObjects[i].color = value;
                }
            }
        }
    }

    void Awake () {
        if(spriteByName == null) {
            spriteByName = new Dictionary<char, Sprite>(15);
            //Debug.Log("Initializing sprite objects");
            for(int i = 0; i < sprites.Count; i++) {
                spriteByName.Add(sprites[i].name.ToCharArray()[0], sprites[i]);
            }
        }
    }

    private void RefreshUI () {
        if (string.IsNullOrEmpty(currentText)) {
            return;
        }

        var splitText = currentText.ToCharArray();
        int limit = Mathf.Min(spriteObjects.Count, splitText.Length);

        for(int i = 0; i < limit; i++) {
            if (spriteByName.ContainsKey(splitText[i])) {
                spriteObjects[i].gameObject.SetActive(true);
                spriteObjects[i].sprite = spriteByName[splitText[i]];
            }
        }

        for(int i = limit; i < spriteObjects.Count; i++) {
            spriteObjects[i].gameObject.SetActive(false);
        }
    }
}
