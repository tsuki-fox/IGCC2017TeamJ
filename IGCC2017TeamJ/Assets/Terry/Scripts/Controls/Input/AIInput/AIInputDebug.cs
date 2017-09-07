using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInputDebug : AIInput {

    private int currentAction = 0;
    [SerializeField]
    private float actionDuration = 3.0f;
    private float actionTimer = 0.0f;

    public int GetCurrentAction() {
        return currentAction;
    }

    public float GetActionDuration() {
        return actionDuration;
    }

    public float GetActionTimer() {
        return actionTimer;
    }

    // Use this for initialization
    void Start () {
        actionTimer = actionDuration;
	}
	
	// Update is called once per frame
	void Update () {
        actionTimer = Mathf.Max(0.0f, actionTimer - Time.deltaTime);

        if (actionTimer <= 0.0f) {
            int numAction = Mathf.Max(booleanActions.Length, axisActions.Length);
            currentAction = (currentAction + 1) % numAction;
            actionTimer = actionDuration;
        }

        for (int i = 0; i < booleanActions.Length; ++i) {
            if (booleanActions[i] == null) {
                continue;
            }

            if (i == currentAction) {
                booleanActions[i].ActivateAction();
            } else {
                booleanActions[i].DeactivateAction();
            }
        }

        for (int i = 0; i < axisActions.Length; ++i) {
            if (axisActions[i] == null) {
                continue;
            }

            if (i == currentAction) {
                axisActions[i].SetInputValue(1.0f);
            } else {
                axisActions[i].SetInputValue(0.0f);
            }
        }
	}

}
