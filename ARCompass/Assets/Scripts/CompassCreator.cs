using UnityEngine;
using TMPro;
using Vuforia;

public class CompassCreator : MonoBehaviour
{
    [Header("Image Recognition")] // Header for unity inspector
    public GameObject imageTarget; // Define marker GameObject public. We are going to get the reference from the Unity inspector.We can also 
    //do private with [SerializeField] tag to get the same result.

    [Header("Compass Creation")] // Header for unity inspector
    public Transform cameraTransform; // Camera's transform component.
    public GameObject compassPrefab; // Compass prefab to Instantiate when we click screen.
    public Transform imageTransform; // Marker's transform component.

    [Header("UI Interaction")] // Header for unity inspector
    public TextMeshProUGUI durumText; // Text component to show in the UI if marker is in screen or not.

    private int counter = 0; // Counter to limit compass amount in the screen.

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) // If touchscreen is touched in mobile phone or tablet.
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position); // Send a ray to the touch point from camera.
            RaycastHit hit; // Define the RaycastHit to find hit location.
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && counter < 2) // If ray hits any location in infinite distance and
            {                                                                   // there is less than 2 compasses in the world.
                if (isTrackingMarker(imageTarget))  //if the marker is in the screen.
                {
                    GameObject obj = Instantiate(compassPrefab,hit.point, Quaternion.identity); // Instantiate compass object in the raycast hit point with the rotation of prefab.

                    switch (counter) 
                    {
                        case 0: // First click counter going to be 0.
                            obj.GetComponentInChildren<TMPro.TextMeshPro>().text = "Kamera"; // Make compass name Kamera.
                            obj.GetComponent<Compass>().compassType = Compass.CompassType.Camera; // Change compass type to Kamera.
                            break;
                        case 1:
                            obj.GetComponentInChildren<TMPro.TextMeshPro>().text = "İşaretçi"; // Make compass name İşaretçi
                            obj.GetComponent<Compass>().compassType = Compass.CompassType.Marker; // Change compass type to Marker.
                            break;
                    }
                    counter++; //Increase counter by one.
                }                 
            }         
        }      
        if (isTrackingMarker(imageTarget)) // If marker is in the screen
        {
            durumText.text = "İşaretçi takip ediliyor..."; //Change the UI text to "Durum: İşaretçi takip ediliyor...".
            durumText.color = Color.green; //Change text color to green
        }
        else  //If markers is not on the screen.
        {
            durumText.text = "İşaretçi kaybedilmiştir!"; //Change the UI text to "Durum: İşaretçi kaybedilmiştir!".
            durumText.color = Color.red; //Change text color to red 
        }
        
    }
    public static bool isTrackingMarker(GameObject imageTarget) // To find if markers in the screen or not.
    {
        var trackable = imageTarget.GetComponent<TrackableBehaviour>(); // Get the TrackableBehavior component of Marker.
        var status = trackable.CurrentStatus; // Get the status of marker if its in the screen or not.
        return status == TrackableBehaviour.Status.TRACKED; //If status returns tracked return this function true.
    }
}
