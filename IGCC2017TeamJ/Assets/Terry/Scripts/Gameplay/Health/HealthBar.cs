using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

	private Health healthComponent;
	private GameObject healthBar;
	private GameObject background;
	private GameObject foreground;

	public bool showWhenFullHealth;
	public Material material;
	public Color backgroundColor, foregroundColor;
	public float backgroundWidth, backgroundHeight;
	public float foregroundWidth, foregroundHeight;
	public Vector3 offset;

	// Use this for initialization
	void Start () {
		healthComponent = gameObject.GetComponent<Health>();

		healthBar = new GameObject();
		healthBar.name = gameObject.name + "'s Health Bar";
		healthBar.AddComponent<Billboard>().reverseDirection = true;

		backgroundWidth = Mathf.Max(0.0f, backgroundWidth);
		backgroundHeight = Mathf.Max(0.0f, backgroundHeight);

		foregroundWidth = Mathf.Max(0.0f, foregroundWidth);
		foregroundHeight = Mathf.Max(0.0f, foregroundHeight);

		//We have to rotate it 180 degrees because Unity's quads faces the wrong way by default.
		Quaternion rotation = new Quaternion();
		rotation.eulerAngles = new Vector3(0, 180, 0);

		background = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Destroy(background.GetComponent<MeshCollider>());
		background.name = "Background";		
		background.transform.SetParent(healthBar.transform);
		background.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		background.transform.localScale = new Vector3(1, 1, 1);
		background.transform.localRotation = rotation;

		foreground = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Destroy(foreground.GetComponent<MeshCollider>());
		foreground.name = "Foreground";
		foreground.transform.SetParent(healthBar.transform);
		foreground.transform.localPosition = new Vector3(0.0f, 0.0f, 0.1f);
		foreground.transform.localScale = new Vector3(1, 1, 1);
		foreground.transform.localRotation = rotation;

		if (material != null) {
			background.GetComponent<MeshRenderer>().material = material;
			foreground.GetComponent<MeshRenderer>().material = material;
		} else {
			print(gameObject.name + "'s Health Bar has no material.");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (healthComponent == null) {
			print(gameObject.name + " has no Health Component. Unable to update Health Bar.");
			return;
		} else if (healthBar == null) {
			print(gameObject.name + " has no Health Bar. Unable to update Health Bar.");
			return;
		} else if (foreground == null || background == null) {
			print(gameObject.name + " has no Background or Foreground. Unable to update Health Bar.");
			return;
		}

		if (showWhenFullHealth == false && healthComponent.IsFullHealth()) {
			healthBar.SetActive(false);
			return;
		} else {
			healthBar.SetActive(true);
		}

		//Set their scales.
		background.transform.localScale = new Vector3(Mathf.Max(0.001f, backgroundWidth), Mathf.Max(0.001f, backgroundHeight), 1.0f);
		foreground.transform.localScale = new Vector3(Mathf.Max(0.001f, foregroundWidth * healthComponent.GetHealthRatio()), Mathf.Max(0.001f, foregroundHeight), 1.0f);

		//Set the positions.
		healthBar.transform.position = gameObject.transform.position + offset;
		foreground.transform.localPosition = new Vector3((foregroundWidth - foreground.transform.localScale.x) * 0.5f, 0.0f, 0.1f);

		//Set their colours.
		MeshRenderer meshRenderer;
		meshRenderer = background.GetComponent<MeshRenderer>();
		if (meshRenderer == null) {
			print(gameObject.name + "'s background has no MeshRenderer.");
		} else {
			background.GetComponent<MeshRenderer>().material.color = backgroundColor;
		}

		meshRenderer = foreground.GetComponent<MeshRenderer>();
		if (meshRenderer == null) {
			print(gameObject.name + "'s foreground has no MeshRenderer.");
		} else {
			foreground.GetComponent<MeshRenderer>().material.color = foregroundColor;
		}
	}

	void OnDestroy() {
		GameObject.Destroy(healthBar);
	}

	void OnEnable() {
		if (healthBar == null) {
			return;
		}
		healthBar.SetActive(true);
	}

	void OnDisable() {
		if (healthBar == null) {
			return;
		}
		healthBar.SetActive(false);
	}

}