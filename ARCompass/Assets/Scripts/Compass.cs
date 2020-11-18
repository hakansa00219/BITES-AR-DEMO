using UnityEngine;
using TMPro;

public class Compass : MonoBehaviour
{
	//Define
	private Transform imageTarget; //Markers transform

	private TextMeshProUGUI trueHeading; // True heading text

	private Vector3 currentRotation; // Current rotation vector of compass object.
	private Vector3 targetRotation; // Target rotation to get north pole direction of compass object.
	private Vector3 markerLocation; // Markers location vector

	private float north; // Compass north pole reading.

	private float duration = 0.2f; // Compass north pole data receive duration.
	private float timeElapsed; // Time spent while Lerping to target angle.

	private float cameraYRotation; // Camera's first y rotation value when we instantiated compass.
	private float targetCameraYRotation; // Camera's variable y rotation value when we move in the room.

	public enum CompassType // Creating enum for Compass Type.
    {
		Camera = 0, // Camera compass shows real north pole direction.
		Marker // Marker compass shows markers view of markers compass to camera.
    }

	public CompassType compassType = CompassType.Camera; // Create a public enum CompassType with default value of 0(Camera)

	private void Awake() // Awake function called first. Call all GetComponent functions in here to get rid of reference problems.
	{
		imageTarget = GameObject.Find("ImageTarget").GetComponent<Transform>();  // Markers transform reference. 
		trueHeading = GameObject.Find("TrueHeading_Text").GetComponent<TextMeshProUGUI>(); // UI object of true heading display.
	}

	void Start()
	{
		cameraYRotation = Camera.main.transform.eulerAngles.y; // Camera start rotation.
		markerLocation = imageTarget.position; // Define markers location vector .
		// Compass created in raycast hit location. Since we are working on flat ground plane we can change compass y location same with 
		// markers y location. Objects need to be in the same height to find the angle of markers compass. 
		transform.position = new Vector3(transform.position.x, markerLocation.y, transform.position.z);

		Input.compass.enabled = true;  //enable compass class from UnityEngine.
		Input.location.Start(); // Start compass north pole detection.

		switch (compassType) // For each compass type
		{
			case CompassType.Camera: // For camera compass type
									 // Angle which we need to turn the compass to get north pole direction.We need to add camera's y angle to the
									 // compass rotation because when we move in real life compass north pole direction needs to stay in the same angle.
				north = Camera.main.transform.eulerAngles.y - Input.compass.trueHeading; 
				break;
			case CompassType.Marker: // For marker compass type
				// To find the angle that markers view of true compass, we need to find the angle difference between true north pole axis and vector towards
				// compass from the marker object.To find that we need to use arctan function and change radian to degree angle.
				float angleToDecrease = Mathf.Atan2((transform.position.x - markerLocation.x), (transform.position.z - markerLocation.z)) * Mathf.Rad2Deg;
				// Add that angle with true compass heading. We dont add camera's y angle because markers compass needs to change with camera location.
				north = Camera.main.transform.eulerAngles.y - Input.compass.trueHeading - angleToDecrease;

				break;
		}

		targetRotation = new Vector3(0f, north, 0f); // Target rotation vector of true markers heading.
		currentRotation = transform.eulerAngles; // Define current rotation of this compass as degrees.
		Rotation(currentRotation, targetRotation, compassType); // Rotate compass first time.
	}
	void Update()
	{
		markerLocation = imageTarget.position; // Update markers location every frame to get good data results for marker compass.
		targetCameraYRotation = Camera.main.transform.eulerAngles.y; // Check if camera is different from start rotation
		trueHeading.text = Input.compass.trueHeading.ToString(); //Update UI Text for compass true heading value.
		//if((compassType == CompassType.Marker && CompassCreator.isTrackingMarker(imageTarget.gameObject)) || compassType == CompassType.Camera) 
		Rotation(currentRotation, targetRotation, compassType); //Call Rotation function to rotate compass.
	}

	// Takes 3 parameters. Current rotation vector of compass, target rotation vector of compass and compass type.
	private void Rotation(Vector3 currentAngle, Vector3 targetAngle,CompassType _compassType)
	{
		// For the defined duration time increase current angle of compass slowly and smoothly to target rotation.
		if (timeElapsed < duration)
		{
			transform.eulerAngles = new Vector3(
			 0f,
			 Mathf.LerpAngle(currentAngle.y, targetAngle.y, timeElapsed / duration),
			 0f);
			timeElapsed += Time.deltaTime;
		}
		else //if duration is over make current rotation same with target rotation. Not always using eulerAngles will make the object rotation
		     //same with target rotation. This way we make sure its same at the end of duration.
		{
			currentRotation = targetRotation;
			// Same switch case part to find true headings of each compass type.
			switch (_compassType)
			{
				case CompassType.Camera:
					north = Camera.main.transform.eulerAngles.y - Input.compass.trueHeading;
					break;
				case CompassType.Marker:
					float angleToDecrease = Mathf.Atan2((transform.position.x - markerLocation.x), (transform.position.z - markerLocation.z)) * Mathf.Rad2Deg;
					// We need to add an extra (targetCameraYRotation - cameraYRotation) formula in here because camera rotation might be changed from our first camera rotation. 
					// If we change camera rotation we also need to change the marker compass rotation the same amount.This way marker compass will always track the same angle that marker's looking.
					north = (targetCameraYRotation - cameraYRotation) + (Camera.main.transform.eulerAngles.y- Input.compass.trueHeading) - angleToDecrease;
					break;
			}
			targetRotation = new Vector3(0, north, 0f); // Target rotation vector of true markers heading.
			timeElapsed = 0; // Make timeElapsed equal to 0 to change the angle of compass every 0.2s(defined duration).
		}
	}
}
