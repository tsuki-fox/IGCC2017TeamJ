using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayChannel : EventChannel {

    private static GameplayChannel instance;

    // Delegates
    public delegate void Void_CharacterDeathInfo(CharacterDeathInfo _info);
    public delegate void Void_GameObject(GameObject _gameObject);

    // Events
    public event Void_CharacterDeathInfo PlayerDeathEvent;
    public event Void_Void ResetItemsEvent;
    public event Void_Bool PlayerHidingEvent;

    public event Void_Void RequestPlayerEvent; // 主人公は誰ですか。
    public event Void_GameObject ReplyPlayerEvent;　// 私！

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

    public void SendRequestPlayerEvent() {
        if (RequestPlayerEvent != null) {
            RequestPlayerEvent();
        }
    }

    public void SendReplyPlayerEvent(GameObject _player) {
        if (ReplyPlayerEvent != null) {
            ReplyPlayerEvent(_player);
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