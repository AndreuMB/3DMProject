using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dron : MonoBehaviour
{
    public GameObject origin;
    public GameObject destiny;
    public Resource resource;
    public Coroutine coroutine;
    Player player;
    Vector3 movingTo;
    public float speed;
    
    public Dron(GameObject origin, GameObject destiny, Resource resource){
        this.origin = origin;
        this.destiny = destiny;
        this.resource = resource;
    }

    public void SetData(GameObject origin, GameObject destiny, Resource resource){
        this.origin = origin;
        this.destiny = destiny;
        this.resource = resource;
        movingTo = destiny.transform.position;
    }

    public float getDistance(){
        return Vector2.Distance(origin.transform.position,destiny.transform.position);
    }

    void Start(){
        player = FindObjectOfType<Player>();
    }

    void Update(){
        float step = speed * Time.deltaTime;
        if (destiny.transform.position == transform.position) {
            movingTo = origin.transform.position;
            transform.LookAt(movingTo);
        }
        if (origin.transform.position == transform.position){
            movingTo = destiny.transform.position;
            transform.LookAt(movingTo);
        }

        transform.position = Vector3.MoveTowards(transform.position, movingTo, step);
    }
}
