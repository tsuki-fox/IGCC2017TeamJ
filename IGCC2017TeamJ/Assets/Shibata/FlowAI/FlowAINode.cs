using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlowAI
{
	public abstract class FlowAINode
	{
		#region private fields
		int _localId = -1;	//ローカルID
		float _duration;	//遷移時間
		
		#endregion

		#region properties
		/// <summary>ローカルID</summary>
		public int localId
		{
			get { return _localId; }
			set { _localId = value; }
		}

		/// <summary>遷移時間</summary>
		public float duration
		{
			get { return _duration; }
			set { _duration = value; }
		}
		#endregion

		#region public methods
		/// <summary>次に遷移するノードを取得する</summary>
		/// <returns>次に遷移するノード,終端ノードの場合はnullを返す</returns>
		public abstract FlowAINode GetNextNode();

		/// <summary>処理を行う</summary>
		public abstract void Processing();
		#endregion
	}
}