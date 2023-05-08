using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float distance;
    [SerializeField] float height;
    [SerializeField] float damping;

    void Start() 
    {
        if (target != GameObject.Find("Player")) 
        {
            target = GameObject.Find("Player");
        }

        if (!tagExists("MainCamera")) 
        {
            throw new UnityException("MainCamera tag does not exist.");
        }
        else if (tagExists("MainCamera") && !this.gameObject.CompareTag("MainCamera")) 
        {
            this.gameObject.tag = "MainCamera";
        }
        else 
        {
            return;
        }

        if (target == null) 
        {
            target = GameObject.FindWithTag("Player");
        }
    }

    void LateUpdate() 
    {
        if (target != null) 
        {
            Transform transformTarget = target.transform;

            Vector3 targetPosition = transformTarget.position - (transformTarget.forward * distance) + (Vector3.up * height);
            Vector3 currentPosition = Vector3.Lerp(transform.position, targetPosition, damping * Time.deltaTime);
            transform.position = currentPosition;
            transform.LookAt(transformTarget.position);
        }
        else 
        {
            throw new MissingComponentException("Target is still null in cameraManager.");
        }
    }

    static bool tagExists(string tag) 
    {
        try 
        {
            GameObject.FindGameObjectsWithTag(tag);
            return true;
        }
        catch 
        {
            return false;
        }
    }
}