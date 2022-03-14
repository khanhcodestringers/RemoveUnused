using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DoOnMainThread : MonoBehaviour {

    private static readonly Queue<Action> tasks = new Queue<Action>();

    public static DoOnMainThread Instance;

    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        this.HandleTasks();
    }

    void HandleTasks()
    {
        while (tasks.Count > 0)
        {
            Action task = null;

            lock (tasks)
            {
                if (tasks.Count > 0)
                {
                    task = tasks.Dequeue();
                }
            }

            task();
        }
    }

    public void QueueOnMainThread(Action task)
    {
        lock (tasks)
        {
            tasks.Enqueue(task);
        }
    }
}
