using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {
    public CurvedLineRenderer curvedLineRenderer;
    public LineRenderer lineRenderer;
    public float segmentLength;
    public float yVariance;

    public CurvedLinePoint[] linePoints;
    public Vector3[] linePositions;

    [SerializeField]
    private float m_speed;
    public float Speed
    {
        set{
            m_speed= Mathf.Clamp(value, 9.5f, 14);
        }
        get{
            return m_speed;
        }
    }

    private void Update() {


        MovePoints();

        DrawMesh();

        CheckPointIfVisible();

    }

    void MovePoints () {
        
        GetPoints();

        for (int i = 0; i < linePoints.Length; i++)
        {
            linePoints[i].transform.Translate(-m_speed * Time.deltaTime, 0, 0);
        }

        ApplyToLineRenderer();

        lineRenderer.positionCount = linePositions.Length;
        lineRenderer.SetPositions(linePositions);
    }

    void GetPoints()
    {
            linePoints = this.GetComponentsInChildren<CurvedLinePoint>();
            linePositions = new Vector3[linePoints.Length];
        for (int i = 0; i < linePoints.Length; i++)
        {
            linePositions[i] = linePoints[i].transform.position;
        }

    }

    void ApplyToLineRenderer () {
        linePositions = LineSmoother.SmoothLine(linePositions,50f);

    }

    // Update is called once per frame
    void AddPoint () {

        Vector3 lastPos = linePoints[linePoints.Length - 1].transform.position;
        float y = Mathf.Clamp(lastPos.y + Random.Range(-yVariance, yVariance), -8, 10);
        float x = Mathf.Clamp(Random.Range(3, 10) + y / 2, 3, 15);
        Vector3 newPosition = new Vector3(lastPos.x + x, y, lastPos.z);
        float angle = Vector3.Angle(lastPos, newPosition);

        GameObject newPoint = ObjectPooler.SharedInstance.GetPooledObject("LevelPoint");
        newPoint.transform.position = newPosition;
        newPoint.SetActive(true);
        newPoint.transform.SetParent(transform);

        // Spawn Gem
        if(angle <= 5)
        {
            int rand = Random.Range(0, 100);

            if (rand > 0 && rand <= 50)
            {
                SpawnGem(newPoint, newPosition);
            }
            if (rand > 50 && rand <= 80)
            {
                if (angle < 2)
                {
                    SpawnSpike(newPoint, newPosition, angle * 2);
                }
            }
        }


	}

   #region Spawn Objects
    void SpawnGem (GameObject point, Vector3 pos) {
        
        bool side = (Random.value > 0.5f);

        GameObject newGem = ObjectPooler.SharedInstance.GetPooledObject("Gem");
        if (newGem != null)
        {
            newGem.transform.position = new Vector3(pos.x, pos.y + (side ? Random.Range(2.5f, 3.5f) : Random.Range(-3.5f, -2.5f)), pos.z);
            newGem.transform.rotation = Quaternion.identity;
            newGem.SetActive(true);
            newGem.transform.SetParent(point.transform, true);
        }

    }

    void SpawnSpike (GameObject point, Vector3 pos, float angle) {
        bool side = (Random.value > 0.5f);

        GameObject newSpike = ObjectPooler.SharedInstance.GetPooledObject("Spike");
        if (newSpike != null)
        {
            newSpike.transform.position = new Vector3(pos.x, pos.y + (side ? 1.5f : -1.5f), pos.z);
            newSpike.transform.rotation = Quaternion.Euler(0, 0, (side ? (0 + angle) : (180 + -angle)));
            newSpike.SetActive(true);
            newSpike.transform.SetParent(point.transform, true);
        }
    }
#endregion

    void CheckPointIfVisible () {
        Vector3 worldLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        if(linePoints[2].transform.position.x <= worldLeft.x)
        {

            foreach (Transform trans in linePoints[0].transform)
            {
                trans.SetParent(null);
            }
            linePoints[0].gameObject.SetActive(false);
            linePoints[0].transform.SetParent(GameManager.instance.UIManager.objectPoolParent);
            AddPoint();
        }
    }

    private void DrawMesh()
    {
        Vector3[] verticies = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(verticies);
        GetComponent<EdgeCollider2D>().points = verticies.toVector2Array();
    }


}

public static class MyVector3Extension
{
    public static Vector2[] toVector2Array(this Vector3[] v3)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(v3, getV3fromV2);
    }

    public static Vector2 getV3fromV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }
}
