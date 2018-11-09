using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class FirstPersonController : MonoBehaviour
{

	private static int LayerMask_Clicky;
	private static int LayerMask_GravitySurface;

	public GameObject player;

	public float rotationSpeed = 5.0f;
	public float walkSpeed = 5.0f;
	public float runSpeed = 10.0f;
	public float jumpSpeed = 5.0f;

	float yaw = 90;
	float yawOffset = 0;
	float pitch = 0;

	private bool jump = false;

	private bool flying = false;
	private Vector3 flyingToPosition;

	private Vector3 targetUp;

	new private Rigidbody rigidbody;

	private Gravity.Direction nearestGravity;

	private GameObject pivot;

	private Grabbable heldObject;
	private GameObject heldObjectAvatar;
	public float holdStrength = 50.0f;
	private float oldHeldBodyAngularDrag;

	void Start ()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		LayerMask_Clicky = (1 << LayerMask.NameToLayer ("Grabbable")) + (1 << LayerMask.NameToLayer ("Button"));
		LayerMask_GravitySurface = 1 << LayerMask.NameToLayer ("GravitySurface");
		rigidbody = GetComponent<Rigidbody> ();
		pivot = new GameObject ("_HeldObjectPivot");
		pivot.transform.SetParent (Camera.main.transform);
		pivot.transform.localPosition = new Vector3 (0, 0, 2);
		targetUp = transform.up;
	}

	void FixedUpdate ()
	{

		yaw += Input.GetAxis ("Mouse X") * rotationSpeed;
		player.transform.localEulerAngles = new Vector3 (player.transform.localEulerAngles.x, yaw + yawOffset, player.transform.localEulerAngles.z);

		pitch -= Input.GetAxis ("Mouse Y") * rotationSpeed;
		pitch = Mathf.Clamp (pitch, -85f, 85f);
		Camera.main.transform.localRotation = Quaternion.Euler (pitch, 0, 0);

		if (flying) {
			
			rigidbody.position = Vector3.Lerp (rigidbody.position, flyingToPosition, 0.1f);

			if ((rigidbody.position - flyingToPosition).magnitude < 1.0f && Vector3.Angle (transform.up, targetUp) < 10)
				flying = false;
			
		} else {

			float speed = Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift) ? runSpeed : walkSpeed;

			float forwardSpeed = Input.GetAxis ("Vertical") * speed;
			float sideSpeed = Input.GetAxis ("Horizontal") * speed;

			#region Follow surface normal
			RaycastHit hit;
			if (Physics.Raycast (transform.position, -targetUp, out hit, 1.0f, LayerMask_GravitySurface) && !hit.normal.Equals (targetUp)) {
				GravitySurface gravitySurface = hit.transform.GetComponent<GravitySurface> ();
				if (gravitySurface != null) {
					targetUp = gravitySurface.getNormal (transform.position);
				} else {
					targetUp = hit.normal;
				}
				calculateNearestGravity ();
			} else {
				targetUp = Gravity.vectors [calculateNearestGravity ()];
			}
			#endregion

			float verticalSpeed;
			if (jump) {
				jump = false;
				verticalSpeed = jumpSpeed;
			} else {
				verticalSpeed = (Quaternion.FromToRotation (targetUp, Vector3.up) * rigidbody.velocity).y;
			}

			Vector3 desiredVelocity = (Quaternion.FromToRotation (transform.up, targetUp) * transform.rotation) * (
			                              player.transform.localRotation
			                              * new Vector3 (
				                              sideSpeed,
				                              verticalSpeed + Physics.gravity.y * Time.fixedDeltaTime,
				                              forwardSpeed
			                              )
			                          );
			Vector3 desiredVelocityChange = desiredVelocity - rigidbody.velocity;
			rigidbody.AddForce (desiredVelocityChange, ForceMode.VelocityChange);

		}

		transform.rotation = Quaternion.Lerp (
			Quaternion.FromToRotation (Vector3.up, Vector3.up),
			Quaternion.FromToRotation (transform.up, targetUp), 0.2f
		) * transform.rotation;

		#region Move heldObjectAvatar
		if (heldObject) {
			heldObject.GetComponent<Rigidbody> ().velocity = holdStrength * (pivot.transform.position - heldObject.transform.position);
			heldObjectAvatar.transform.position = heldObject.IsColliding ? heldObject.transform.position : pivot.transform.position;
			if (heldObject.IsColliding) {
				heldObjectAvatar.transform.rotation = heldObject.transform.rotation;
			}
		}
		#endregion
		
	}

	void Update ()
	{
		#region Jump
		if (isGrounded () && Input.GetButtonDown ("Jump")) {
			jump = true;
		}
		#endregion
		#region Teleport to surface
		RaycastHit hit;
		if (Input.GetMouseButtonDown (1) && Physics.Raycast (GetLookingAt (), out hit, 99f, 1 << LayerMask.NameToLayer ("GravitySurface"))) {
			flyingToPosition = hit.point;
			flying = true;
			targetUp = hit.normal;
		}
		#endregion
		#region Throw held object
		if (Input.GetMouseButtonDown (0)) {
			if (heldObject) {
				throwHeldObject ();
			} else {

			}
		}
		#endregion
		#region Interact (push button, pick up object, put down object, etc.)
		if (Input.GetKeyDown (KeyCode.E)) {
			if (heldObject) {
				ungrab ();
			} else if (Physics.Raycast (GetLookingAt (), out hit, 4.0f, LayerMask_Clicky)) {
				if (hit.transform.gameObject.GetComponent<Grabbable> ()) {
					grab (hit.transform.gameObject.GetComponent<Grabbable> ());
				} else if (hit.transform.gameObject.GetComponent<Button> ()) {
					hit.transform.gameObject.GetComponent<Button> ().Press ();
				}
			}
		}
		#endregion

	}

	bool isGrounded ()
	{
		return Physics.Raycast (transform.position, -targetUp, 0.51f);
	}

	#region Grabbing things

	void grab (Grabbable thing)
	{
		calculateNearestGravity ();
		if (thing == null) {
			if (heldObject != null)
				ungrab ();
		} else {
			heldObject = thing;

			heldObjectAvatar = new GameObject ("_HeldBodyAvatar");
			heldObjectAvatar.transform.parent = Camera.main.transform;
			heldObjectAvatar.transform.position = heldObject.transform.position;
			heldObjectAvatar.transform.rotation = heldObject.transform.rotation;
			heldObjectAvatar.transform.localScale = heldObject.transform.localScale;
			heldObjectAvatar.AddComponent<MeshFilter> ().sharedMesh = heldObject.gameObject.GetComponent<MeshFilter> ().sharedMesh;
			heldObjectAvatar.AddComponent<MeshRenderer> ().sharedMaterials = heldObject.gameObject.GetComponent<Renderer> ().sharedMaterials;

			heldObject.grab (heldObjectAvatar.GetComponent<Renderer> ());
			heldObject.transform.position = pivot.transform.position;
			oldHeldBodyAngularDrag = heldObject.GetComponent<Rigidbody> ().angularDrag;
			heldObject.GetComponent<Rigidbody> ().angularDrag = 1;
			heldObject.GetComponent<Renderer> ().enabled = false;

			Physics.IgnoreCollision (heldObject.GetComponent<Collider> (), GetComponent<Collider> (), true);
		}
	}

	void throwHeldObject ()
	{
		Grabbable thing = heldObject;
		ungrab ();
		thing.GetComponent<Rigidbody> ().AddForce (Camera.main.transform.forward * 10, ForceMode.Impulse);
	}

	void ungrab ()
	{
		heldObject.ungrab ();
		Physics.IgnoreCollision (heldObject.GetComponent<Collider> (), GetComponent<Collider> (), false);
		Rigidbody heldRB = heldObject.GetComponent<Rigidbody> ();
		heldRB.angularDrag = oldHeldBodyAngularDrag;
		heldRB.transform.position = heldObjectAvatar.transform.position;
		heldRB.transform.rotation = heldObjectAvatar.transform.rotation;
		heldRB.velocity = rigidbody.velocity;
		heldRB.GetComponent<Renderer> ().enabled = true;
		GameObject.Destroy (heldObjectAvatar);
		heldObject = null;
	}

	#endregion

	Gravity.Direction calculateNearestGravity ()
	{
		Gravity.Direction newNearestGravity = nearestGravity;
		float distanceToClosest = 180;
		foreach (Gravity.Direction grav in new Gravity.Direction[] {
			Gravity.Direction.Down,
			Gravity.Direction.Up,
			Gravity.Direction.Left,
			Gravity.Direction.Right,
			Gravity.Direction.Front,
			Gravity.Direction.Back,
		}) {
			float vectorDistance = Vector3.Angle (Gravity.vectors [grav], targetUp);
			if (vectorDistance < distanceToClosest) {
				distanceToClosest = vectorDistance;
				newNearestGravity = grav;
			}
		}
		if (!nearestGravity.Equals (newNearestGravity) && heldObject != null) {
			heldObject.gravityTransition (nearestGravity, newNearestGravity);
			heldObjectAvatar.GetComponent<MeshRenderer> ().sharedMaterials = heldObject.gameObject.GetComponent<Renderer> ().sharedMaterials;
		}
		return nearestGravity = newNearestGravity;
	}

	Ray GetLookingAt ()
	{
		return new Ray (Camera.main.transform.position, Camera.main.transform.forward);
	}

}
