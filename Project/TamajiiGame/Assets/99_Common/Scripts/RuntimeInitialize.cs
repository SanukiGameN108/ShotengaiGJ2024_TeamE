using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeInitialize
{
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        BGM.Create();
    }
}
