using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChannel : EventChannel {

    private static PlayerChannel instance;

    // Events
    public event Void_Void PlayerDeathEvent;
    public event Void_Void ResetItemsEvent;

    // Event Sending
    public void SendPlayerDeathEvent() {
        PlayerDeathEvent();
    }

    public void SendResetItemsEvent() {
        ResetItemsEvent();
    }

    private PlayerChannel() {
    }

    public static PlayerChannel GetInstance() {
        if (instance == null) {
            instance = new PlayerChannel();
        }

        return instance;
    }

}
