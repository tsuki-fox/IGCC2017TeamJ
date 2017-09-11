using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FlowAI;

namespace FlowAI
{
	public class FlowAIVisualizer : MonoBehaviour
	{
		struct DrawnData
		{
			public FlowAINode node;
			public Rect rect;

			public DrawnData(FlowAINode node ,Rect rect)
			{
				this.node = node;
				this.rect = rect;
			}
		}


		[SerializeField]
		FlowAIBasis _targetBasis = null;
		[SerializeField]
		Texture2D _processTex;
		[SerializeField]
		Texture2D _branchTex;
		[SerializeField]
		Texture2D _arrowHeadTex;
		[SerializeField]
		Texture2D _lineTex;
		[SerializeField]
		float _nodeSpace = 20f;
		[SerializeField]
		Vector2 _beginArrowOffset = Vector2.zero;
		[SerializeField]
		Vector2 _endArrowOffset = Vector2.zero;
		[SerializeField]
		Vector2 _branchBeginArrowOffset = Vector2.zero;
		[SerializeField]
		Vector2 _branchEndArrowOffset = Vector2.zero;

		[SerializeField]
		float _lineThickness = 1f;
		[SerializeField]
		Vector2 _lineOffset = Vector2.zero;

		[SerializeField]
		Vector2 _nodeSize = new Vector2(100f,33f);
		[SerializeField]
		float _nodeSizeScale = 1f;

		List<DrawnData> _drawnNode = new List<DrawnData>();

		private void Start()
		{
			_targetBasis = FindObjectOfType<EnemyControl>()._flowAI;
		}

		void DrawNode(FlowAINode node,Vector2 pos)
		{
			//描画済みノードならば終了
			if (_drawnNode.Exists(item => item.node == node))
				return;

			Rect nodeRect = new Rect(pos, _nodeSize * _nodeSizeScale);

			//処理ノード及びエントリーノード
			if(node is ProcessNode || node is FlowAIBasis.EntryPointNode)
			{
				_drawnNode.Add(new DrawnData(node, nodeRect));

				if (node == _targetBasis.currentNode)
				{
					var temp = GUI.color;
					GUI.color = Color.red;
					GUI.DrawTexture(nodeRect, _processTex);
					GUI.color = temp;
				}
				else
				{
					GUI.DrawTexture(nodeRect, _processTex);
				}

				GUIStyle style = new GUIStyle();
				GUIStyleState styleState = new GUIStyleState();
				style.fontSize = (int)(_nodeSize.y * _nodeSizeScale);
				styleState.textColor = Color.white;
				style.normal = styleState;

				GUI.Label(nodeRect, "LID:" + node.localId, style);
				

				if(node.GetNextNode()!=null)
				{
					DrawNode(node.GetNextNode(), pos + new Vector2(0, _nodeSize.y + _nodeSpace));
				}
			}

			//分岐ノード
			else if(node is BranchNode)
			{
				var branch = node as BranchNode;

				_drawnNode.Add(new DrawnData(node, nodeRect));

				if (branch == _targetBasis.currentNode)
				{
					var temp = GUI.color;
					GUI.color = Color.red;
					GUI.DrawTexture(nodeRect, _branchTex);
					GUI.color = temp;
				}
				else
				{
					GUI.DrawTexture(nodeRect, _branchTex);
				}

				GUIStyle style = new GUIStyle();
				GUIStyleState styleState = new GUIStyleState();
				style.fontSize = (int)(_nodeSize.y * _nodeSizeScale);
				styleState.textColor = Color.white;
				style.normal = styleState;

				GUI.Label(nodeRect, "LID:" + node.localId, style);

				if (branch.trueNode != null)
				{
					DrawNode(branch.trueNode, pos + new Vector2(0, _nodeSize.y+_nodeSpace));
				}

				if(branch.falseNode!=null)
				{
					DrawNode(branch.falseNode, pos + new Vector2(_nodeSpace, _nodeSize.y + _nodeSpace));
				}
			}
		}

		void DrawLines()
		{
			foreach (var drawn in _drawnNode)
			{
				if (drawn.node is ProcessNode || drawn.node is FlowAIBasis.EntryPointNode)
				{
					//始点
					Vector2 begin = drawn.rect.position + new Vector2(drawn.rect.width / 2f, drawn.rect.height);
					Rect beginRect = new Rect(begin+_beginArrowOffset, new Vector2(10f, 10f));
					GUI.DrawTexture(beginRect, _arrowHeadTex);

					//終点
					Vector2 end = drawn.rect.position + new Vector2(drawn.rect.width / 2f, drawn.rect.height + _nodeSpace);
					Rect endRect = new Rect(end + _endArrowOffset, new Vector2(10f, 10f));
					GUI.DrawTexture(endRect, _arrowHeadTex);

					//線
					if (drawn.node == _targetBasis.currentNode)
					{
						Rect lineRect = new Rect(begin + _lineOffset, new Vector2(_lineThickness, end.y - begin.y));

						var temp = GUI.color;
						GUI.color = Color.red;
						GUI.DrawTexture(lineRect, _lineTex);
						GUI.color = temp;
					}
					else
					{
						Rect lineRect = new Rect(begin + _lineOffset, new Vector2(_lineThickness, end.y - begin.y));
						GUI.DrawTexture(lineRect, _lineTex);
					}
				}

				else if(drawn.node is BranchNode)
				{
					var branch = drawn.node as BranchNode;

					//始点
					Vector2 trueBegin = drawn.rect.position + new Vector2(drawn.rect.width / 2f, drawn.rect.height);
					Rect trueBeginRect = new Rect(trueBegin + _beginArrowOffset, new Vector2(10f, 10f));
					GUI.DrawTexture(trueBeginRect, _arrowHeadTex);

					//終点
					Vector2 trueEnd = drawn.rect.position + new Vector2(drawn.rect.width / 2f, drawn.rect.height + _nodeSpace);
					Rect trueEndRect = new Rect(trueEnd + _endArrowOffset, new Vector2(10f, 10f));
					GUI.DrawTexture(trueEndRect, _arrowHeadTex);

					//線
					if (branch == _targetBasis.currentNode && branch.GetNextNode() == branch.trueNode)
					{
						Rect lineRect = new Rect(trueBegin + _lineOffset, new Vector2(_lineThickness, trueEnd.y - trueBegin.y));

						var temp = GUI.color;
						GUI.color = Color.red;
						GUI.DrawTexture(lineRect, _lineTex);
						GUI.color = temp;
					}
					else
					{
						Rect lineRect = new Rect(trueBegin + _lineOffset, new Vector2(_lineThickness, trueEnd.y - trueBegin.y));
						GUI.DrawTexture(lineRect, _lineTex);
					}

					//分岐始点
					Vector2 falseBegin = drawn.rect.position + new Vector2(drawn.rect.width, drawn.rect.height / 2f);
					Rect falseBeginRect = new Rect(falseBegin + _branchBeginArrowOffset, new Vector2(10f, 10f));

					GUIUtility.RotateAroundPivot(-90f, falseBegin);
					GUI.DrawTexture(falseBeginRect, _arrowHeadTex);
					GUI.matrix = Matrix4x4.identity;

					//分岐終点
					var falseNodeDrawnData = _drawnNode.Find(item => item.node == branch.falseNode);

					Vector2 falseEnd = falseNodeDrawnData.rect.position + new Vector2(_nodeSize.x / 2f, -_nodeSpace / 2f);
					Rect falseEndRect = new Rect(falseEnd + _branchEndArrowOffset, new Vector2(10f, 10f));

					GUIUtility.RotateAroundPivot(90f, falseEnd);
					GUI.DrawTexture(falseEndRect, _arrowHeadTex);
					GUI.matrix = Matrix4x4.identity;
				}
			}
		}

		private void OnGUI()
		{
			GUI.Box(new Rect(new Vector2(5f, 5f), new Vector2(1000, 1000)), "");

			_drawnNode.Clear();
			DrawNode(_targetBasis.entryPointNode, new Vector2(10f, 10f));

			DrawLines();
		}
	}
}