using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NoteTouchEffect : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    Animator animator;
    void Start () {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnTouchDown (Vector2 position) {
        transform.position = position;
        animator.SetBool("show", true);
    }

    public void OnTouchUp () {
        animator.SetBool("show", false);
    }

    public NoteTouchEffect Instantiate () {
        NoteTouchEffect re = GameObject.Instantiate(this);
        re.name = name;
        re.Start();
        re.transform.parent = transform.parent;
        return re;
    }

    public void OnAnimationShowStart () {
        spriteRenderer.enabled = true;
    }

    public void OnAnimationHideComplete () {
        spriteRenderer.enabled = false;
    }


}

public class NoteTouchEffectManagement {

    protected Camera noteCamera;
    protected NoteTouchEffect baseEffectScript;
    Queue<NoteTouchEffect> queEffect = new Queue<NoteTouchEffect>();
    Dictionary<GameObject, NoteTouchEffect> dicEffect = new Dictionary<GameObject, NoteTouchEffect>();

    public NoteTouchEffectManagement (Camera noteCamera, NoteTouchEffect noteMultiTouchEffectScript) {
        this.noteCamera = noteCamera;
        baseEffectScript = noteMultiTouchEffectScript;
        queEffect.Enqueue(noteMultiTouchEffectScript);
    }

    public void OnTouchDown (GameObject note, TouchCover touch) {
        //if (note != null) {
        NoteTouchEffect effectScript = queEffect.Count > 0 ? queEffect.Dequeue() : baseEffectScript.Instantiate();
        effectScript.OnTouchDown(noteCamera.ScreenToWorldPoint(touch.position));
        if (!dicEffect.ContainsKey(note)) {
            dicEffect.Add(note, effectScript);
        }
        //}
    }

    public void OnTouchUp (GameObject note) {
        if (dicEffect.ContainsKey(note) == false)
            return;

        NoteTouchEffect effectScript = dicEffect[note];
        dicEffect.Remove(note);
        effectScript.OnTouchUp();
        queEffect.Enqueue(effectScript);
    }

    public void OnGameReset () {
        Vector2 position = noteCamera.transform.position;
        position.x -= 100 + baseEffectScript.transform.lossyScale.x;
        foreach (NoteTouchEffect item in queEffect)
            item.transform.position = position;
    }
}



