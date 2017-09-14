using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FlowAI;

public class TestTullet : MonoBehaviour {
    public FlowAIBasis _flowAI;

    bool _isRot = false;
    bool _isBullet = false;
    bool _isFound = false;
 
    public float interval = 1.0f;

    private float time = 0.0f;

    private Vector3 lastKnownPlayerPosition;

    bool canSeePlayerPrevious = false;

    Animator animator;

    // VisionCone
    [SerializeField]
    private VisionCone _visionCone;

    private GameObject Player = null;

    [SerializeField]
    private GameObject head;


    // Shooting
    [SerializeField]
    private GameObject bulletObject;
    [SerializeField, Range(0.0f, 999999.0f)]
    private float fireRate = 0.5f;
    private float fireTimer = 0.0f;
    [SerializeField]
    private Vector3 bulletSpawnOffset = new Vector3(0, 0, 1);


    void InitEvents()
    {
        GameplayChannel.GetInstance().ReplyPlayerEvent += ReplyPlayerEvent;
    }

    void DeinitEvents()
    {
        GameplayChannel.GetInstance().ReplyPlayerEvent -= ReplyPlayerEvent;
    }

    void ReplyPlayerEvent(GameObject _player)
    {
        Player = _player;
    }

    // Use this for initialization
    void Start()
    {
        //FlowAI生成 Create FlowAI.
        _flowAI = GetComponent<FlowAIHolder>().flowAI;

        //ノード生成 Create nodes.
        var foundBranch = new BranchNode();
        var missingBranch = new  BranchNode();

        var foundBranch2 = new BranchNode();
        var missingBranch2 = new BranchNode();

        // 旋回
        var rotNode = new ProcessNode();
        var stopRotNode = new ProcessNode();

        // 攻撃
        var ShootingNode = new ProcessNode();
        var StopShootingNode = new ProcessNode();


        foundBranch.summary = "見てる?";
        missingBranch.summary = "見てない";

        foundBranch2.summary = "打てる?";
        missingBranch2.summary = "打てない";

        rotNode.summary = "まわる";
        stopRotNode.summary = "回らない";

        ShootingNode.summary = "打ちます";
        StopShootingNode.summary = "打つのやめます";
    

        //ノード初期化 Initialize nodes.
        rotNode.Initialize(0.1f, foundBranch, () => _isRot = true);
        stopRotNode.Initialize(0.1f, ShootingNode, () => _isRot = false);

        ShootingNode.Initialize(0.1f, foundBranch2, () => _isBullet = true);
        StopShootingNode.Initialize(0.1f, rotNode   , () => _isBullet = false);

        foundBranch.Initialize(stopRotNode, 0.1f, rotNode, 0.1f, () => _isFound);
        missingBranch.Initialize(rotNode, 0.1f, stopRotNode, 0.1f, () => !_isFound);

        foundBranch2.Initialize(ShootingNode, 0.1f, StopShootingNode, 0.1f, () => _isFound);
        missingBranch2.Initialize(StopShootingNode, 0.1f, ShootingNode, 0.1f, () => !_isFound);

        //ノード追加 Add nodes at FlowAIBasis.
        _flowAI.AddNode(rotNode, foundBranch, missingBranch, stopRotNode, ShootingNode ,StopShootingNode, foundBranch2, missingBranch2);

        //エントリポイントの次のノードを設定 Setting next node for entry point node.
        _flowAI.entryPointNode.nextNode = rotNode;

        //AI開始 Transition entry point.
        _flowAI.Entry();

        animator = GetComponent<Animator>();

        //animator.SetTrigger("Shot");
    }

    void Awake()
    {
        // VisionCone生成
        _visionCone.CreateVisionConeObject(gameObject);
        //_visionCone.set

        // イベント初期化呼び出し
        InitEvents();
    }

    // Update is called once per frame
    void Update()
    {
        fireTimer = Mathf.Max(0.0f, fireTimer - Time.deltaTime);

        // プレイヤーの情報が何も入ってない場合
        if (Player == null) {
            GameplayChannel.GetInstance().SendRequestPlayerEvent();
            return;
        }

        //更新処理 Update.
        _flowAI.Update(Time.deltaTime);
        
        time += Time.deltaTime;

        bool canSeePlayer = UpdatePlayerLastKnownPosition();

        _visionCone.CreateVisionConeMesh();

        // 敵を見ているかの判定
        //if (_visionCone.IsTargetInVisionCone(gameObject, Player))
        if (CanSeePlayer())
        {
            _isFound = true;

            //Debug.Log("Can see target! 主人公を見ています！");

        }
        else
        {
            _isFound = false;

            //Debug.Log("Cannot see target! 主人公を見ていません！");
        }
      
      
        //Any process...
        if (_isRot)
        {
            
            //head.transform.Rotate(new Vector3(0,0, 60f * Time.deltaTime));
            this.transform.Rotate(new Vector3(0,60f * Time.deltaTime));
            head.transform.Rotate(new Vector3(0, (((-1)*60f) * Time.deltaTime)));
        }
        if(_isBullet)
        {

            Shoot();
        }

        canSeePlayerPrevious = canSeePlayer;

        TFDebug.ClearMonitor("enemy");
        TFDebug.Write("enemy", "isRot:{0}\n", _isRot.ToString());
        TFDebug.Write("enemy", "isFound:{0}\n", _isFound.ToString());
    }

    private void OnDestroy()
    {
        DeinitEvents();
    }

    public bool CanSeePlayer()
    {
        Assert.AreNotEqual(Player, null);

        bool result = _visionCone.IsTargetInVisionCone(gameObject, Player);
        if (result)
        {
            // 主人公が見えます。
            // しかし、それは隠れているかもしれません。(ブッシュ)
          
            Player playerComponent = Player.GetComponent<Player>();
            Assert.AreNotEqual(playerComponent, null);

            // 前のフレームでプレイヤーを見たら、それが茂みに入るのがわかりました。
            if (playerComponent.IsHiding() & !canSeePlayerPrevious)
            {
                return false;
            }
        }

        return result;
    }

    bool UpdatePlayerLastKnownPosition()
    {
        bool result = CanSeePlayer();

        if (result)
        {
            lastKnownPlayerPosition = Player.transform.position;
            GameplayChannel.GetInstance().SendPlayerSpottedEvent(lastKnownPlayerPosition);
        }

        return result;
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - gameObject.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 rotationEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        rotationEulerAngles.y = rotation.eulerAngles.y;
        rotation.eulerAngles = rotationEulerAngles;
        gameObject.transform.rotation = rotation;
    }

    void Shoot()
    {
        Assert.AreNotEqual(bulletObject, null);

        // Stop moving and look at the player.
        //MoveTowards(gameObject.transform.position, 0.0f);
        RotateTowards(lastKnownPlayerPosition);
        if (fireTimer <= 0.0f)
        {
            GameObject bullet = GameObject.Instantiate(bulletObject);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            animator.SetTrigger("Shot");
            Assert.AreNotEqual(bulletComponent, null);

            bullet.transform.position = transform.position;
            bullet.transform.position += bulletSpawnOffset.x * transform.right;
            bullet.transform.position += bulletSpawnOffset.y * transform.up;
            bullet.transform.position += bulletSpawnOffset.z * transform.forward;
            bullet.transform.rotation = transform.rotation;

            fireTimer = 1.0f / fireRate;
        }
    }
}
