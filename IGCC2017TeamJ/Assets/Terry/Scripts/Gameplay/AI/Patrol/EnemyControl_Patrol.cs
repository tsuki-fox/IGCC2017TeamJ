using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using FlowAI;

public class EnemyControl_Patrol : MonoBehaviour
{
	public FlowAIBasis flowAI;

    // For spotting the player.
    [SerializeField]
    private VisionCone visionCone;
    private GameObject player = null;

    // Patrolling
    [SerializeField]
    private List<Transform> waypoints;
    private int activeWaypoint = 0;

    // Movement Values
    [SerializeField]
    private float patrolVelocity = 5.0f;
    [SerializeField]
    private float chaseVelocity = 10.0f;
    
    enum AttackType {
        AttackType_Shoot,
        AttackType_Explode
    }

    [SerializeField]
    private AttackType attackType = AttackType.AttackType_Shoot;

    [SerializeField]
    private float attackRange = 5.0f;

    // Explosion
    [SerializeField]
    private GameObject explosionObject;
    [SerializeField]
    private int explosionDamage = 50;
    [SerializeField]
    private float explosionRange = 12.0f;
    [SerializeField]
    private List<string> explosionBlockerTags;

    // Shooting
    [SerializeField]
    private GameObject bulletObject;
    [SerializeField, Range(0.0f, 999999.0f)]
    private float fireRate = 0.5f;
    private float fireTimer = 0.0f;
    [SerializeField]
    private Vector3 bulletSpawnOffset = new Vector3(0, 0, 1);

    // 到達するノードにどれくらい近づける必要があるか。
    // How close to the node we need to be to be considered reaching it.
    [SerializeField]
    private float distanceToWaypoint = 1.0f;

    // 主人公最後の既知の位置
    // Where our AI will go to when chasing the player.
    private Vector3 lastKnownPlayerPosition;
    
    // あきらめる (追)
    [SerializeField, Range(0.1f, 99999.0f)]
    private float chaseGiveUpDuration = 8.0f;
    private float chaseGiveUpTimer = 8.0f;

    [SerializeField]
    // あなたは距離を見ることができます (パトロール)
    private float patrolViewDistance = 10.0f;
    [SerializeField]
    // あなたは角度を見ることができます (パトロール)
    private float patrolViewAngle = 30.0f;
    [SerializeField]
    // あなたは距離を見ることができます (追)
    private float combatViewDistance = 20.0f;
    // あなたは角度を見ることができます （追）
    [SerializeField]
    private float combatViewAngle = 60.0f;

    [SerializeField]
    // 主人公お見ていますのMaterial
    private Material canSeePlayerMaterial;
    [SerializeField]
    // 主人公お見ていませんのMaterial　(追）
    private Material chaseCannotSeePlayerMaterial;
    // 主人公お見ていませんのMaterial (パトロール)
    [SerializeField]
    private Material patrolCannotSeePlayerMaterial;

    // 前のフレーム主人公おみましたか？
    // For detecting if we saw the player enter the hiding spot (e.g. Bush).
    private bool canSeePlayerPrevious = false;

    // For alerts
    private int chaseNodeId;
    [SerializeField]
    private bool ignoreAlert = false;
    // We will receive the alert if we are less or equal to this distance away from the player.
    [SerializeField, Range(0.0f, 100.0f)]
    private float alertDistance = 50.0f;

    // State Machine
    enum AIState {
        AIState_None,
        AIState_Patrol,
        AIState_Chase,
        AIState_Explode,
        AIState_Shoot,
    }

    // 現のフレームの状態
    private AIState currentState = AIState.AIState_None;
    // 前のフレームの状態
    private AIState previousState = AIState.AIState_None;

    void InitEvents() {
        GameplayChannel.GetInstance().ReplyPlayerEvent += ReplyPlayerEvent;
        GameplayChannel.GetInstance().PlayerSpottedEvent += PlayerSpottedEvent;
    }

    void DeinitEvents() {
        GameplayChannel.GetInstance().ReplyPlayerEvent -= ReplyPlayerEvent;
        GameplayChannel.GetInstance().PlayerSpottedEvent -= PlayerSpottedEvent;
    }

    void ReplyPlayerEvent(GameObject _player) {
        player = _player;
    }

    void PlayerSpottedEvent(Vector3 _playerPosition) {
        if (ignoreAlert) {
            return;
        }
        if (GameplayHelper.GetSquaredDistanceBetween(_playerPosition, gameObject.transform.position) > alertDistance * alertDistance) {
            return;
        }

        //Debug.Log("PlayerSpottedEvent");
        lastKnownPlayerPosition = _playerPosition;

        // We already see the player.
        if (canSeePlayerPrevious) {
            return;
        }
        
        chaseGiveUpTimer = chaseGiveUpDuration;
       // flowAI.Swap(chaseNodeId, flowAI.currentNode.localId);
        flowAI.Transition(chaseNodeId);
    }

	// Use this for initialization
	void Start () {
        //FlowAI生成 Create FlowAI.
        Assert.AreNotEqual(GetComponent<FlowAIHolder>(), null);
        flowAI = GetComponent<FlowAIHolder>().flowAI;

        ProcessNode chaseNode = new ProcessNode();
        ProcessNode explodeNode = new ProcessNode();
        ProcessNode shootNode = new ProcessNode();
        ProcessNode alertNode = new ProcessNode();
        ProcessNode patrolMoveNode = new ProcessNode();
        ProcessNode patrolReachedNode = new ProcessNode();

        BranchNode canSeePlayerNode = new BranchNode();
        BranchNode playerInAttackRangeNode = new BranchNode();
        BranchNode checkReachedWaypointNode = new BranchNode();
        BranchNode checkPlayerEscapedNode = new BranchNode();

        canSeePlayerNode.Initialize(alertNode, 0.05f, checkReachedWaypointNode, 0.05f, CanSeePlayer, "Can See Player?");
        alertNode.Initialize(0.05f, playerInAttackRangeNode, AlertNodeFunction, "Alert Others");
        checkReachedWaypointNode.Initialize(patrolReachedNode, 0.05f, patrolMoveNode, 0.05f, CheckReachedWaypoint, "Reached Waypoint?");
        
        chaseNode.Initialize(0.05f, checkPlayerEscapedNode, ChaseNodeFunction, "Chase Target");
        checkPlayerEscapedNode.Initialize(canSeePlayerNode, 0.05f, playerInAttackRangeNode, 0.05f, CheckPlayerEscaped, "Has Target Escaped?");

        patrolMoveNode.Initialize(0.05f, canSeePlayerNode, PatrolMoveNodeFunction, "Patrol Move");
        patrolReachedNode.Initialize(0.05f, canSeePlayerNode, PatrolReachedNodeFunction, "Patrol Reached Waypoint");

        switch (attackType) {
            case AttackType.AttackType_Explode:
                explodeNode.Initialize(0.05f, null, ExplodeNodeFunction, "Explosion!");
                playerInAttackRangeNode.Initialize(explodeNode, 0.05f, chaseNode, 0.05f, PlayerInAttackRange, "Is Target In Explosion Range?");
                flowAI.AddNode(explodeNode);
                break;
            case AttackType.AttackType_Shoot:
                shootNode.Initialize(1.0f, playerInAttackRangeNode, ShootNodeFunction, "Shoot!");
                playerInAttackRangeNode.Initialize(shootNode, 0.05f, chaseNode, 0.05f, PlayerInAttackRange, "Is Target In Shooting Range?");
                flowAI.AddNode(shootNode);
                break;
            default:
                break;
        }
        

        flowAI.AddNode(alertNode, patrolMoveNode, patrolReachedNode, chaseNode, canSeePlayerNode, playerInAttackRangeNode, checkReachedWaypointNode, checkPlayerEscapedNode);

        //エントリポイントの次のノードを設定 Setting next node for entry point node.
        flowAI.entryPointNode.nextNode = canSeePlayerNode;

        //AI開始 Transition entry point.
        flowAI.Entry();

        chaseNodeId = chaseNode.localId;

        // Initialise events.
        InitEvents();

        chaseGiveUpTimer = chaseGiveUpDuration;
        visionCone.SetMaterial(patrolCannotSeePlayerMaterial);
        visionCone.CreateVisionConeObject(gameObject);
    }

    private void OnDestroy() {
        DeinitEvents();
    }

    // Update is called once per frame
    void Update ()
	{
        fireTimer = Mathf.Max(0.0f, fireTimer - Time.deltaTime);

        //Check if we know who the player is.
        if (player == null) {
            //Debug.Log("EnemyControl_Patrol::Update - No Player!");
            GameplayChannel.GetInstance().SendRequestPlayerEvent(); //主人公は誰ですか。
            return; // Skip this frame.
        }

        // 現のフレーム主人公おみましたか？
        bool canSeePlayer = UpdatePlayerLastKnownPosition();

        flowAI.Update(Time.deltaTime);

        switch (currentState) {
            case AIState.AIState_Patrol:
                Patrol();
                //Debug.Log(gameObject.name + "'s current state is PATROL.");
                break;
            case AIState.AIState_Chase:
                //Debug.Log(gameObject.name + "'s current state is CHASE.");
                Chase(canSeePlayer);
                break;
            case AIState.AIState_Explode:
                //Debug.Log(gameObject.name + "'s current state is EXPLODE.");
                エクスプロージョン();
                break;
            case AIState.AIState_Shoot:
                Shoot();
                break;
            default:
                // Do nothing.
                break;
        }

        if (canSeePlayer != canSeePlayerPrevious || previousState != currentState) {
            if (canSeePlayer) {
                visionCone.SetMaterial(canSeePlayerMaterial, true);
            } else {
                if (currentState == AIState.AIState_Chase) {
                    visionCone.SetMaterial(chaseCannotSeePlayerMaterial, true);
                } else {
                    visionCone.SetMaterial(patrolCannotSeePlayerMaterial, true);
                }
            }
        }

        // 更新
        // Update
        previousState = currentState;
        canSeePlayerPrevious = canSeePlayer;
	}

    int IncreaseWaypointIndex() {
        if (waypoints.Count >= 0) {
            activeWaypoint = (activeWaypoint + 1) % waypoints.Count;
        } else {
            activeWaypoint = 0;
        }

        return activeWaypoint;
    }

    // 巡回
    void Patrol() {
        //Debug.Log("Patrol");

        visionCone.SetViewAngle(patrolViewAngle);
        visionCone.SetViewDistance(patrolViewDistance, true);

        // Nowhere to go.
        if (waypoints.Count == 0) {
            return;
        }

        // Make sure the waypoint isn't null.
        // Assert.AreNotEqual(null, waypoints[activeWaypoint]);
        if (waypoints[activeWaypoint] == null) {
            return;
        }

        Vector3 destination = waypoints[activeWaypoint].position;
        Vector3 currentPos = transform.position;

        MoveTowards(destination, patrolVelocity);
        RotateTowards(destination);
    }

    // 追
    void Chase(bool canSeePlayer) {
        visionCone.SetViewAngle(combatViewAngle);
        visionCone.SetViewDistance(combatViewDistance, true);

        // Give up the chase if we run out of time and cannot see the player.
        if (canSeePlayer) {
            chaseGiveUpTimer = chaseGiveUpDuration;
        } else if ((chaseGiveUpTimer -= Time.deltaTime) < 0.0f) {
            //Debug.Log("Gave up chase!");
            return;
        }

        MoveTowards(lastKnownPlayerPosition, chaseVelocity);
        RotateTowards(lastKnownPlayerPosition);
    }

    /*「黒より黒く闇より暗き漆黒に我が深紅の混淆を望みたもう。
       覚醒のとき来たれり。無謬の境界に落ちし理。
       無行の歪みとなりて現出せよ！踊れ踊れ踊れ、我が力の奔流に望むは崩壊なり。
       並ぶ者なき崩壊なり。万象等しく灰塵に帰し、深淵より来たれ！
       これが人類最大の威力の攻撃手段、これこそが究極の攻撃魔法、エクスプロージョン！」*/
    void エクスプロージョン() {
        //Debug.Log("エクスプロージョン");
        // Create an explosion.
        GameObject explosion = GameObject.Instantiate(explosionObject);
        explosion.transform.position = gameObject.transform.position;

        // Deal damage to the player.
        Vector3 directionToPlayer = player.transform.position - gameObject.transform.position;
        if (directionToPlayer.sqrMagnitude <= explosionRange * explosionRange) {
            // Raycast to ensure that nothing is blocking the explosion.
            if (!GameplayHelper.HasObstaclesBetween(gameObject.transform.position, player.transform.position, explosionBlockerTags)) {
                Health playerHealth = player.GetComponent<Health>();
                //Assert.AreNotEqual(playerHealth, null);
                if (playerHealth != null) {
                    playerHealth.DecreaseHealth(explosionDamage);
                }
            }
        }

        // Kill this gameobject.
        gameObject.SetActive(false);
    }

    void Shoot() {
        Assert.AreNotEqual(bulletObject, null);

        // Stop moving and look at the player.
        MoveTowards(gameObject.transform.position, 0.0f);
        RotateTowards(lastKnownPlayerPosition);
        if (fireTimer <= 0.0f) {
            GameObject bullet = GameObject.Instantiate(bulletObject);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            Assert.AreNotEqual(bulletComponent, null);

            bullet.transform.position = transform.position;
            bullet.transform.position += bulletSpawnOffset.x * transform.right;
            bullet.transform.position += bulletSpawnOffset.y * transform.up;
            bullet.transform.position += bulletSpawnOffset.z * transform.forward;
            bullet.transform.rotation = transform.rotation;

            fireTimer = 1.0f / fireRate;
        }
    }

    // Helper Functions
    void MoveTowards(Vector3 _destination, float _speed) {
        NavMeshAgent navAgent = gameObject.GetComponent<NavMeshAgent>();
        //Assert.AreNotEqual(navAgent, null);
        if (navAgent == null) {
            return;
        }
        navAgent.SetDestination(_destination);
        navAgent.speed = _speed;
    }
    
    void RotateTowards(Vector3 targetPosition) {
        Vector3 direction = targetPosition - gameObject.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        Vector3 rotationEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        rotationEulerAngles.y = rotation.eulerAngles.y;
        rotation.eulerAngles = rotationEulerAngles;
        gameObject.transform.rotation = rotation;
    }

    // 最後に主人公を見た場所。
    bool UpdatePlayerLastKnownPosition() {
        bool result = CanSeePlayer();

        if (result) {
            lastKnownPlayerPosition = player.transform.position;
            GameplayChannel.GetInstance().SendPlayerSpottedEvent(lastKnownPlayerPosition);
        }

        return result;
    }

    // Branch Node Predicated
    public bool CanSeePlayer() {
        //Assert.AreNotEqual(player, null);
        if (player == null) {
            return false;
        }
        
        bool result = visionCone.IsTargetInVisionCone(gameObject, player);
        if (result) {
            // 主人公が見えます。
            // しかし、それは隠れているかもしれません。(ブッシュ)
            // https://www.youtube.com/watch?v=aoY6ulcQ7r4
            Player playerComponent = player.GetComponent<Player>();
            Assert.AreNotEqual(playerComponent, null);

            // 前のフレームでプレイヤーを見たら、それが茂みに入るのがわかりました。
            if (playerComponent.IsHiding() & !canSeePlayerPrevious) {
                return false;
            }
        }
        
        return result;
    }

    bool PlayerInAttackRange() {
        //Debug.Log("PlayerInAttackRange");
        Assert.AreNotEqual(player, null);

        if (CanSeePlayer()) {
            return GameplayHelper.GetSquaredDistanceBetween(lastKnownPlayerPosition, gameObject.transform.position) < attackRange * attackRange;
        }

        return false;
    }

    bool CheckReachedWaypoint() {
        //Debug.Log("CheckReachedWaypoint");
        if (waypoints.Count == 0 || activeWaypoint >=waypoints.Count) {
            return true;
        }

        // There should not be any empty waypoints!
        //Assert.AreNotEqual(waypoints[activeWaypoint], null);
        if (waypoints[activeWaypoint] == null) {
            return true;
        }

        Vector3 currentPos = transform.position;
        Vector3 destination = waypoints[activeWaypoint].transform.position;
        
        if (GameplayHelper.GetSquaredDistanceBetween(currentPos, destination) < distanceToWaypoint * distanceToWaypoint) {
            return true;
        }

        return false;
    }
    
    bool CheckPlayerEscaped() {
        //Debug.Log("CheckPlayerEscaped");
        return chaseGiveUpTimer <= 0.0f && !CanSeePlayer();
    }

    // アラート
    // Process Node Functions
    void AlertNodeFunction() {
        //Debug.Log("AlertNodeFunction");
        currentState = AIState.AIState_None;
        GameplayChannel.GetInstance().SendPlayerSpottedEvent(lastKnownPlayerPosition);
    }

    // 巡回
    void PatrolMoveNodeFunction() {
        //Debug.Log("PatrolMoveNodeFunction");
        // Doot dii doot dii doo, just strolling along..
        currentState = AIState.AIState_Patrol;
    }

    // 巡回
    void PatrolReachedNodeFunction() {
        //Debug.Log("PatrolReachedNodeFunction");
        IncreaseWaypointIndex();
    }

    // 追
    void ChaseNodeFunction() {
        //Debug.Log("ChaseNodeFunction");
        currentState = AIState.AIState_Chase;
    }

    // 爆
    void ExplodeNodeFunction() {
        //Debug.Log("ExplodeNodeFunction");
        currentState = AIState.AIState_Explode;
    }

    // 射
    void ShootNodeFunction() {
        //Debug.Log("ShootNodeFunction");
        visionCone.SetViewAngle(combatViewAngle);
        visionCone.SetViewDistance(combatViewDistance, true);
        currentState = AIState.AIState_Shoot;
    }

}