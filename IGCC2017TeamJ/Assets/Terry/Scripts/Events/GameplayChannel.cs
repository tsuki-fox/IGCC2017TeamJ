using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayChannel : EventChannel {

    private static GameplayChannel instance;

    // Delegates
    public delegate void Void_CharacterDeathInfo(CharacterDeathInfo _info);

    // Events
    public event Void_CharacterDeathInfo PlayerDeathEvent;
    public event Void_Void ResetItemsEvent;
    public event Void_Bool PlayerHidingEvent;

    // Event Sending
    public void SendPlayerDeathEvent(CharacterDeathInfo _info) {
        if (PlayerDeathEvent != null) {
            PlayerDeathEvent(_info);
        }
    }

    public void SendResetItemsEvent() {
        if (ResetItemsEvent != null) {
            ResetItemsEvent();
        }
    }
    
    public void SendPlayerHidingEvent(bool _isHiding) {
        if (PlayerHidingEvent != null) {
            PlayerHidingEvent(_isHiding);
        }
    }

    private GameplayChannel() {
    }

    public static GameplayChannel GetInstance() {
        if (instance == null) {
            instance = new GameplayChannel();
        }

        return instance;
    }

}