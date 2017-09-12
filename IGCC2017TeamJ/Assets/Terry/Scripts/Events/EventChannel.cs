using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChannel {

    public delegate void Void_Void();
    public delegate void Void_Int(int _int);
    public delegate void Void_Float(float _float);
    public delegate bool Bool_IntInt(int _int0, int _int1);
    public delegate void Void_Bool(bool _bool);

    protected EventChannel() {
    }

}