using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuple<TypeA, TypeB> {
	
	public TypeA first;
	public TypeB second;

	public Tuple() {
        first = default(TypeA);
        second = default(TypeB);
	}

	public Tuple(TypeA _first, TypeB _second) {
        first = _first;
        second = _second;
	}

}