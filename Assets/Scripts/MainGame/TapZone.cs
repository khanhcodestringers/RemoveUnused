using UnityEngine;
using System.Collections;
using Mio.TileMaster;
//using Framework;
using DG.Tweening;

[RequireComponent(typeof(Collider2D))]
public class TapZone : MonoBehaviour
{
    [SerializeField] protected int _lineIndex;

#if UNITY_EDITOR || UNITY_WEBGL
    [SerializeField] protected KeyCode _inputPress;
#endif

    protected NoteSimple _noteStay = null;
    public NoteSimple NoteStay { get { return _noteStay; } }

    private void Update()
    {
        if (Input.GetKeyDown(_inputPress))
        {
            IsTap = true;
            IsHolding = false;

            if (_noteStay && _noteStay is NoteSimple)
            { 
                _noteStay.Press()
            }
        }

        if (Input.GetKey(_inputPress))
        {
            holdTime += Time.deltaTime;

            if (HOLDING_TIME < holdTime)
            {
                IsHolding = true;
            }
        }
        if (Input.GetKeyUp(_inputPress))
        {
            IsHolding = false;
            IsTap = false;
        }
    }

    public static float HOLDING_TIME = 0.0f;
    protected float holdTime;

    protected bool IsTap = false;
    protected bool IsHolding = false;

    public void OnTriggerEnter2D(Collider2D other)
    {
        var note = other.GetComponent<NoteSimple>();
        
        if (note)
        {
            _noteStay = note;
            Debug.Log(this.name + " : " + other.name + " -- " + other.GetHashCode());

        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var note = other.GetComponent<NoteSimple>();
        if (note && note.Equals(_noteStay))
            Debug.LogError(this.name + " : " + other.name + " -- " + other.GetHashCode());
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        var note = other.GetComponent<NoteSimple>();

        if (note && note.Equals(_noteStay))
        {
            _noteStay = null;
        }
        else
        {
            // maybe has another enter.
        }
    }
}
