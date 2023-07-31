using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TargetFinder))]    
public class Citizen : AlkylEntity
{
    public float fallSpeed;     //Speed moving towards planet
    public float angle;         //Angle to planet IN DEGREES
    public float radius;        //Distance to planet
    public float planetRadius;   //How close to the planet is the ground?
    public float bounceRadius;   //Used for bounce animation
    public float gravityRate;
    public Planet nearestPlanet;
    public float lastBehavior;
    public float walkTime;
    public float walkState;
    public bool taggedForCapture;

    TargetFinder finder;

    public override void Awake() {
        base.Awake();
        finder = GetComponent<TargetFinder>();
    }

    public override void StateMachineSetup(){
        StateMachine.BindModeToFunctions(this);
    }

    public override void Start() {
        base.Start();

        CensusTaker.Instance.citizens.Add(this);
    }
    public void OnDestroy() {
        CensusTaker.Instance.citizens.Remove(this);
    }
    /*
    * Mode list
    * 0 = no planet
    * 1 = found planet
    * 2 = captured by lander
    */

    public void State_Floating_0() {
        FindPlanet();
        transform.Rotate(new Vector3(0, 0, 1));
    }

    public void State_Wandering_0() {
        if(!nearestPlanet) {
            Destroy(gameObject);
            return;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        transform.position = nearestPlanet.transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0) * (radius + bounceRadius);
        if(radius > planetRadius) {
            bounceRadius = 0;
            walkState = 0;
            lastBehavior = GameManager.Instance.unpausedTime;
            fallSpeed += gravityRate * Time.deltaTime;
            radius -= fallSpeed * Time.deltaTime;
            if(radius <= planetRadius) {
                fallSpeed = 0;
                radius = planetRadius;
            }
        } else {
            if(radius < planetRadius) { //Prevent going into the planet
                radius = planetRadius;
            }
            
            if(bounceRadius > 0) { //If off the ground in animation...
                fallSpeed += gravityRate * Time.deltaTime;
            } else { //If on the ground, can switch behaviors
                fallSpeed = 0;
                bounceRadius = 0;
                if (GameManager.Instance.timeSince(lastBehavior) > walkTime) {
                    walkState = walkState != 0 ? 0 : Mathf.Sign(Random.Range(-1, 1));
                    lastBehavior = GameManager.Instance.unpausedTime + Random.Range(-0.2f, 0.2f);
                }
            }

            if (walkState != 0) { //Move if behavior is set to walking
                angle += walkState * 0.5f;
                if (bounceRadius == 0) {
                    fallSpeed = -1.5f;
                    bounceRadius = 0.01f;
                }
            }

            bounceRadius -= fallSpeed * Time.deltaTime;
        }
    }

    public void State_Captured_0() {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        nearestPlanet = null;
    }


    void FindPlanet() {
        Transform target = finder.GetClosestTarget();
        
        if (!target) {
            Mode = 0;
            bounceRadius = 0;
            return;
        }

        if(nearestPlanet != null) //Check if we've swapped for a new planet for some reason
            if(target != nearestPlanet.transform) { 
                Mode = 0;
            }

        nearestPlanet = target.GetComponent<Planet>();
        if (Mode == 0) { //Transition from floating to falling towards a planet
            radius = Vector2.Distance(transform.position.ConvertTo2D(), target.position.ConvertTo2D());
            planetRadius = 2f;
            float y = transform.position.y - target.transform.position.y; 
            float x = transform.position.x - target.transform.position.x;
            angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            Mode = 1;
        }
    }
}
