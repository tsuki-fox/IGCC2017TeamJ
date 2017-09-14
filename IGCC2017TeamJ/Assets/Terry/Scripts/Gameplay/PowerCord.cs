using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using FlowAI;

public class PowerCord : MonoBehaviour {

    public float duration = 1.0f;
    public float timer = 0.0f;
    public float distance = 5.0f;
    public List<string> hackableTags;

    private bool isHacking = false;

	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable() {
        timer = 0.0f;
        isHacking = false;
    }

    void UpdateLocalScale() {
        Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        timer += Time.deltaTime;
        scale.z = Mathf.Sin((timer / duration) * Mathf.PI) * distance;
        transform.localScale = scale;
    }

    void UpdateLocalPosition() {
        Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
        position.z = transform.localScale.z * 0.5f;
        transform.localPosition = position;
    }

    void UpdateHackingCheck() {
        RaycastHit[] result = Physics.RaycastAll(transform.position, transform.forward, transform.localScale.z);
        for (int i = 0; i < result.Length; ++i) {
            Collider hitCollider = result[i].collider;
            GameObject hitGameObject = hitCollider.gameObject;

            for (int j = 0; j < hackableTags.Count; ++j) {
                if (hitGameObject.tag == hackableTags[j]) {
                    FlowAIHolder flowAIHolder = hitGameObject.GetComponent<FlowAIHolder>();
                    if (flowAIHolder == null) {
                        break;
                    }

                    {
                        // Can't hack if they can see us.
                        EnemyControl_Patrol enemyControlPatrol = hitGameObject.GetComponent<EnemyControl_Patrol>();
                        if (enemyControlPatrol != null && enemyControlPatrol.CanSeePlayer()) {
                            break;
                        }
                    }

                    {
                        // Check Turret here.
                    }

                    // Do hacking here.
                    Debug.Log("Hacked");

                    FlowAIVisualizer visualizer = FindObjectOfType<FlowAI.FlowAIVisualizer>();
                    Assert.AreNotEqual(visualizer, null);
                    visualizer.target = flowAIHolder;

                    visualizer.BeginHacking(1.0f);

                    gameObject.SetActive(false);

                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (isHacking) {
            return;
        }

        UpdateLocalScale();
        UpdateLocalPosition();
        UpdateHackingCheck();

        if (timer > duration) {
            gameObject.SetActive(false);
        }
	}

}
