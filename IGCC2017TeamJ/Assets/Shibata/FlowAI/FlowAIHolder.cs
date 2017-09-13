using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowAI;

public class FlowAIHolder : MonoBehaviour
{
	FlowAIBasis _flowAI;

	public FlowAIBasis flowAI
	{
		get { return _flowAI; }
		set { _flowAI = value; }
	}

	void Awake()
	{
		_flowAI = new FlowAIBasis();
	}
}
