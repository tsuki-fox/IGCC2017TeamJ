using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using FlowAI;

namespace FlowAI
{
	public class FlowAIVisualizer : MonoBehaviour
	{
		struct PrepareData
		{
			public FlowAINode node;
			public int depthX;
			public int depthY;
			public bool isActive;

			public PrepareData(FlowAINode node,int depthX,int depthY,bool isActive)
			{
				this.node = node;
				this.depthX = depthX;
				this.depthY = depthY;
				this.isActive = isActive;
			}
		}

		#region private fields
		[SerializeField]
		FlowAIHolder _target;		//ターゲットAIホルダ
		FlowAIBasis _targetBasis;	//ターゲットAI

		[SerializeField]
		Texture2D _processTex;		//処理ノードテクスチャ
		[SerializeField]
		Texture2D _branchTex;		//分岐ノードテクスチャ
		[SerializeField]
		Texture2D _arrowHeadTex;	//矢印テクスチャ
		[SerializeField]
		Texture2D _lineTex;         //線テクスチャ

		[SerializeField]
		Color _nodeColor = Color.white;
		[SerializeField]
		Color _activeNodeColor = Color.red;

		[SerializeField]
		Vector2 _windowPosition;    //ウィンドウの位置
		[SerializeField]
		Vector2 _windowSize;		//ウィンドウのサイズ
		[SerializeField]
		Vector2 _globalOffset;		//全体のオフセット

		[SerializeField]
		float _nodeVerticalSpace = 20f;		//ノード間の垂直方向の間隔
		[SerializeField]
		float _nodeHorizontalSpace = 20f;	//ノード間の水平方向の間隔
		[SerializeField]
		Vector2 _nodeSize = new Vector2(100f, 33f); //ノードの大きさ

		[SerializeField]
		float _lineWidth = 3f;					//線の太さ
		[SerializeField]
		Color _lineColor = Color.white;         //線の色
		[SerializeField]
		Color _activeLineColor = Color.red;     //アクティブ時の線の色

		List<PrepareData> _prepares = new List<PrepareData>();  //準備済みリスト

		[SerializeField]
		bool _isVisible = true;	//表示フラグ
		#endregion

		#region properties
		public FlowAIHolder target { get { return _target; } set { _target = value; } }
		public bool isVisible { get { return _isVisible; } set { _isVisible = value; } }
		#endregion

		void Prepare(FlowAINode node)
		{
			_prepares.Clear();
			Prepare(node, 0, 0);
		}

		void Prepare(FlowAINode node,int depthX,int depthY)
		{
			//準備済みノードならば終了
			if (_prepares.Exists(item => item.node == node))
				return;

			//準備済みリストに追加
			_prepares.Add(new PrepareData(node, depthX, depthY, _targetBasis.currentNode == node));

			//処理ノードかエントリポイントノードならば
			if(node is ProcessNode || node is FlowAIBasis.EntryPointNode)
			{
				//Y方向の深さを1つ掘る
				Prepare(node.GetNextNode(), depthX, depthY + 1);
				return;
			}

			//分岐ノードならば
			else if(node is BranchNode)
			{
				var branch = node as BranchNode;

				//Y方向の深さを1つ掘る
				Prepare(branch.trueNode, depthX, depthY + 1);
				//X方向とY方向の深さを1つ掘る
				Prepare(branch.falseNode, depthX + 1, depthY + 1);
				return;
			}
		}

		/// <summary>ノードを描画する</summary>
		void DrawNodes()
		{
			foreach (var item in _prepares)
			{
				//位置
				Vector2 pos = new Vector2(item.depthX * _nodeHorizontalSpace, item.depthY * _nodeVerticalSpace);
				pos += _globalOffset;

				//矩形
				Rect nodeRect = new Rect(Vector2.zero, _nodeSize);
				nodeRect.center = pos;

				if (item.isActive)
				{
					float progress = _targetBasis.elapsed / item.node.duration;
					EditorGUI.ProgressBar(nodeRect, progress, "");
				}

				if (item.isActive)
					GUI.color = _activeNodeColor;
				else
					GUI.color = _nodeColor;

				//処理ノード及びエントリノードの描画
				if (item.node is ProcessNode || item.node is FlowAIBasis.EntryPointNode)
				{
					GUI.DrawTexture(nodeRect, _processTex);
				}
				//分岐ノードの描画
				else if(item.node is BranchNode)
				{
					GUI.DrawTexture(nodeRect, _branchTex);
				}

				GUI.Label(nodeRect, item.node.summary);

				if (item.isActive)
					GUI.color = Color.white;
			}
		}

		void DrawBezier(Vector2 begin, Vector2 end, PrepareData from, PrepareData to,Color color)
		{
			//同じノードなら
			if(from.node==to.node)
			{
				Handles.DrawBezier(begin, end,
						begin + new Vector2(_nodeSize.x, _nodeSize.y * 3f),
						end + new Vector2(_nodeSize.x, -_nodeSize.y * 3f),
						color, _lineTex, _lineWidth);
				return;
			}

			//始点よりもYが深ければ
			if (to.depthY > from.depthY)
			{
				//Xが同じ深さ
				if (from.depthX == to.depthX)
				{
					Handles.DrawBezier(begin, end,
						begin - new Vector2(_nodeSize.x, 0f),
						end + new Vector2(_nodeSize.x, 0f),
						color, _lineTex, _lineWidth);
				}
				//右
				else if (from.depthX < to.depthY)
				{
					Handles.DrawBezier(begin, end,
						begin + new Vector2(_nodeSize.x, 0f),
						end,
						color, _lineTex, _lineWidth);
				}
				//左
				else
				{
					Handles.DrawBezier(begin, end,
						begin,
						end - new Vector2(_nodeSize.x, 0f),
						color, _lineTex, _lineWidth);
				}

			}
			//始点よりもYが浅ければ
			else
			{
				//Xが同じ深さ
				if (from.depthX == to.depthX)
				{
					Handles.DrawBezier(begin, end,
						begin - new Vector2(_nodeSize.x, -_nodeSize.y * 3f),
						end - new Vector2(_nodeSize.x, _nodeSize.y * 3f),
						color, _lineTex, _lineWidth);
				}
				//右
				else if (from.depthX < to.depthY)
				{
					Handles.DrawBezier(begin, end,
						begin - new Vector2(_nodeSize.x, 0f),
						end - new Vector2(_nodeSize.x, 0f),
						color, _lineTex, _lineWidth);
				}
				//左
				else
				{
					Handles.DrawBezier(begin, end,
						begin + new Vector2(_nodeSize.x * 2f, _nodeSize.y * 3f),
						end + new Vector2(_nodeSize.x, -_nodeSize.y * 3f),
						color, _lineTex, _lineWidth);
				}
			}
		}

		void DrawLines()
		{
			foreach(var item in _prepares)
			{
				//位置
				Vector2 pos = new Vector2(item.depthX * _nodeHorizontalSpace, item.depthY * _nodeVerticalSpace);
				pos += _globalOffset;

				if (item.node is ProcessNode || item.node is FlowAIBasis.EntryPointNode)
				{
					var next = _prepares.Find(elem => elem.node == item.node.GetNextNode());
					var nextPos = new Vector2(next.depthX * _nodeHorizontalSpace, next.depthY * _nodeVerticalSpace);
					nextPos += _globalOffset;

					Vector2 begin = new Vector2(pos.x, pos.y + _nodeSize.y / 2f);
					Vector2 end = new Vector2(nextPos.x, nextPos.y - _nodeSize.y / 2f);

					if (item.isActive)
						DrawBezier(begin, end, item, next, _activeLineColor);
					else
						DrawBezier(begin, end, item, next, _lineColor);
				}
				else if(item.node is BranchNode)
				{
					var branch = item.node as BranchNode;

					{
						var trueNext = _prepares.Find(elem => elem.node == branch.trueNode);
						var trueNextPos = new Vector2(trueNext.depthX * _nodeHorizontalSpace, trueNext.depthY * _nodeVerticalSpace);
						trueNextPos += _globalOffset;

						Vector2 begin = new Vector2(pos.x, pos.y + _nodeSize.y / 2f);
						Vector2 end = new Vector2(trueNextPos.x, trueNextPos.y - _nodeSize.y / 2f);

						if (item.isActive && branch.GetNextNode() == branch.trueNode)
							DrawBezier(begin, end, item, trueNext, _activeLineColor);
						else
							DrawBezier(begin, end, item, trueNext, _lineColor);
					}
					{
						var falseNext = _prepares.Find(elem => elem.node == branch.falseNode);
						var falseNextPos = new Vector2(falseNext.depthX * _nodeHorizontalSpace, falseNext.depthY * _nodeVerticalSpace);
						falseNextPos += _globalOffset;

						Vector2 begin = new Vector2(pos.x + _nodeSize.x / 2f, pos.y);
						Vector2 end = new Vector2(falseNextPos.x, falseNextPos.y - _nodeSize.y / 2f);

						if (item.isActive && branch.GetNextNode() == branch.falseNode)
							DrawBezier(begin, end, item, falseNext, _activeLineColor);
						else
							DrawBezier(begin, end, item, falseNext, _lineColor);
					}
				}
			}

		}

		void Start()
		{
			if (_target != null)
				_targetBasis = _target.GetComponent<FlowAIHolder>().flowAI;
		}

		void OnValidate()
		{
			if (_target != null)
				_targetBasis = _target.GetComponent<FlowAIHolder>().flowAI;
			else
				_targetBasis = null;
		}

		void OnGUI()
		{
			//非表示
			if (!_isVisible)
				return;

			GUI.Box(new Rect(_windowPosition, _windowSize), "");

			//ターゲットにするAIがnull
			if (_targetBasis == null)
			{
				var temp = GUI.color;
				GUI.color = Color.red;
				GUI.Label(new Rect(_windowPosition + _globalOffset, new Vector2(100f, 100f)), "Target AI not found");
				GUI.color = temp;
				return;
			}

			Prepare(_targetBasis.entryPointNode);
			DrawNodes();
			DrawLines();
		}
	}
}