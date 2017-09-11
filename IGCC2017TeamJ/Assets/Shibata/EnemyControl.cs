﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowAI;

public class EnemyControl : MonoBehaviour
{

	public FlowAIBasis _flowAI;

	bool _isRot = false;
	bool _isFound = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "Player")
			_isFound = true;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.name == "Player")
			_isFound = false;
	}

	// Use this for initialization
	void Awake ()
	{
		//FlowAI初期化
		_flowAI = new FlowAIBasis();

		//ノード生成
		var rotNode = new ProcessNode();
		var foundBranch = new BranchNode();
		var missingBranch = new BranchNode();

		var alertNode = new ProcessNode();
		var stopAlertNode = new ProcessNode();
		var stopRotNode = new ProcessNode();

		//ノード初期化
		rotNode.Initialize(0.1f, foundBranch, () => _isRot = true);
		stopRotNode.Initialize(0.1f, missingBranch, () => _isRot = false);

		foundBranch.Initialize(alertNode, 0.1f, foundBranch, 0.1f, () => _isFound);
		missingBranch.Initialize(stopAlertNode, 0.1f, missingBranch, 0.1f, () => !_isFound);

		alertNode.Initialize(0.1f, stopRotNode, () => Debug.Log("Alert!"));
		stopAlertNode.Initialize(0.1f, rotNode, () => Debug.Log("Alert stopped"));

		//ノード追加
		_flowAI.AddNode(rotNode, foundBranch, missingBranch, alertNode, stopAlertNode, stopRotNode);

		//エントリポイントの次のノードを設定
		_flowAI.entryPointNode.nextNode = rotNode;

		//AI開始
		_flowAI.Entry();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//更新処理
		_flowAI.Update(Time.deltaTime);

		if (_isRot)
		{
			this.transform.Rotate(new Vector3(0f, 60f * Time.deltaTime));
		}

		TFDebug.ClearMonitor("enemy");
		TFDebug.Write("enemy", "isRot:{0}\n",_isRot.ToString());
		TFDebug.Write("enemy", "isFound:{0}\n", _isFound.ToString());
	}
}
