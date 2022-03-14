using UnityEngine;
using System.Collections;

public class ControllPump : MonoBehaviour
{

    public TweenPosition tweenPos;
    public TweenRotation tweenRotate;
    //public static ControllPump main;
    //// Use this for initialization
    //void Start () {
    //    main = this;
    //}
    public void ShowPump() 
    {
        tweenPos.PlayForward();
    }
    public void HidePump()
    {
        tweenPos.PlayReverse();
    }
}
