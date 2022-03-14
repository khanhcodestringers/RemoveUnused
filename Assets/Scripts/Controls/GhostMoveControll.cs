using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GhostMoveControll : MonoBehaviour
{
    public float movementSpeed;
    bool onMove;
    public List<Transform> dotMoves;

    void OnEnable() 
    {
        StartCoroutine(IERandomMove());
    }

    void OnDisable() 
    {
        StopCoroutine(IERandomMove());
    }

    IEnumerator IERandomMove()
    {
        yield return new WaitForSeconds(Random.Range(5f, 10f));
        if (onMove) yield break;
        Transform dotTF = dotMoves[Random.Range(0, dotMoves.Count)];
        Vector3 vec;
        if (transform.position.x < dotTF.position.x && transform.rotation.y != 180)
        {
            vec = Vector3.zero;
            vec.y = 180;
            transform.rotation = Quaternion.Euler(vec);
        }
        else if (transform.position.x > dotTF.position.x && transform.rotation.y != 0)
        {
            vec = Vector3.zero;
            //vec.y = 180;
            transform.rotation = Quaternion.Euler(vec);
        }

        transform.DOMove(dotTF.position, 3f).OnStart(() =>
        {
            onMove = true;
        }).OnComplete(() =>
        {
            onMove = false;
        }).Play();

        
        StartCoroutine(IERandomMove());
    }
}