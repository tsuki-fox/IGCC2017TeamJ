using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChannel {

    public delegate void Void_Void();
    public delegate void Void_Int(int _int);
    public delegate void Void_Float(float _float);

    protected EventChannel() {
    }

}