using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour {
    public static PlanetSpawner Instance;

    public GameObject PlanetObject;
    public GameObject CitizenObject;
    public Vector2 MinMaxPlanetScale;
    public Vector2 MinMaxPlanetDistance;
    public int NumberOfPlanets;
    public int CitizensPerPlanet;

    public List<Transform> Planets {get; private set; }

    Vector2 GetPlanetPosition(Vector2 lastPlanetPosition, float lastPlanetSize, float planetSize) {
        begin:
        Vector2 pos = lastPlanetPosition + (Random.insideUnitCircle.normalized * (Random.Range(MinMaxPlanetDistance.x, MinMaxPlanetDistance.y) + (lastPlanetSize / 2f) + (planetSize / 2f)));
        pos.x = pos.x.Wrap(Playfield.Instance.Settings.TopLeftCorner.x, Playfield.Instance.Settings.BottomRightCorner.x);
        pos.y = pos.y.Wrap(Playfield.Instance.Settings.BottomRightCorner.y, Playfield.Instance.Settings.TopLeftCorner.y);

        //high chance for an infinite loop here if the planet counts is too high
        //We'll play it by ear and figure out exactly how many planets we can have
        for(int i = 0; i < Planets.Count; i++) {
            if(Vector2.Distance(Planets[i].position, pos) <= MinMaxPlanetDistance.x) {
                //I may implement a  max count variable so it just kinda gives up after a certain number of tries
                goto begin;
            }
        }

        return pos;
    }

    GameObject CreatePlanet(Vector2 position) {
        GameObject currentPlanet = Instantiate(PlanetObject, position, Quaternion.identity, transform);
        float scale = 1;
        currentPlanet.transform.localScale = new Vector2(scale, scale);
        for(int i = 0; i < CitizensPerPlanet; i++) {
            Citizen citizen = Instantiate(CitizenObject, position, Quaternion.identity, transform).GetComponent<Citizen>();
            citizen.angle = Random.Range(0, 360);
            citizen.radius = scale*2;
            citizen.planetRadius = scale*2;
        }
        return currentPlanet;
    }

    private void Awake() {
        Instance = this;
        Planets = new List<Transform>();
        Vector2 size = Playfield.Instance.PlayfieldSize / 2f;
        Planets.Add(CreatePlanet(Random.insideUnitCircle * size).transform);

        for(int i = 0; i < NumberOfPlanets - 1; i++) {
            Planets.Add(
                CreatePlanet(
                    GetPlanetPosition(
                        Planets[Planets.Count - 1].position, 
                        Planets[Planets.Count - 1].localScale.x, 
                        Random.Range(MinMaxPlanetScale.x, MinMaxPlanetScale.y)
                    )
                ).transform
            );
        }
    }
}
