using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransition
{
    void Play();
    void Stop();
    void Skip();

    event Action<ITransition> OnStarted;
    event Action<ITransition> OnFinished;
}
