﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowAI
{
	public abstract class FlowAINode
	{
		#region protected fields
		protected int _localId = -1;	//ローカルID
		protected float _duration;    //遷移時間
		protected string _summary;	//ノードの概要説明用文字列
		
		#endregion

		#region properties
		/// <summary>
		/// ローカルID
		/// Local ID.
		/// </summary>
		public int localId
		{
			get { return _localId; }
			set { _localId = value; }
		}

		/// <summary>
		/// 遷移時間
		/// Duration for transition.
		/// </summary>
		public float duration
		{
			get { return _duration; }
			set { _duration = value; }
		}

		/// <summary>
		/// ノード概要の説明用文字列
		/// characters of node summary.
		/// </summary>
		public string summary
		{
			get { return _summary; }
			set { _summary = value; }
		}
		#endregion

		#region public methods
		/// <summary>
		/// 次に遷移するノードを取得する
		/// Get next transition node.
		/// </summary>
		/// <returns>次に遷移するノード,終端ノードの場合はnullを返す</returns>
		public abstract FlowAINode GetNextNode();

		/// <summary>
		/// 処理を行う
		/// Processing.
		/// </summary>
		public abstract void Processing();
		#endregion
	}
}