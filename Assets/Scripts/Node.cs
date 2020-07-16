using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Node : MonoBehaviour
{
    internal int nState;
    internal float baseDistance;

    internal int id;
    internal int status;

    internal SpriteRenderer spriteRenderer;

    public readonly List<Node> neighbours = new List<Node>();
    public bool debug = true;

    public GameObject shadowPrefab;


    #region MonoBehaviour

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetStatus();

        CreateShadow();
    }

    private void Update()
    {
        UpdatePosition();

        UpdateEdge();

        SetSize();
    }

    internal void Step()
    {
        ChangeStatus();

        foreach (Node n in neighbours)
        {
            n.ChangeStatus();
        }

        GameManager.instance.ClearLevel();
    }

    internal void StepQuiet()
    {
        ChangeStatus();

        foreach (Node n in neighbours)
        {
            n.ChangeStatus();
        }
    }
    #endregion



    #region Positioning

    // force based position adjustment
    //  * https://csacademy.com/app/graph_editor
    //  * https://en.wikipedia.org/wiki/Graph_drawing

    private void UpdatePosition()
    {
        Vector3 curVel = Vector3.zero;

        foreach (Node n in neighbours)
        {
            ApplyForce(n.transform.position, transform.position, ref curVel);
        }
        ApplyForce(Vector3.zero, transform.position, ref curVel);

        transform.position += curVel;
    }

    private void UpdateEdge()
    {
        RemoveEdge();

        DrawEdge();
    }

    private void ApplyForce(Vector3 p1, Vector3 p2, ref Vector3 curVel)
    {
        float distance = Vector3.Distance(p1, p2);

        Vector3 direction = Vector3.Normalize(p1 - p2);

        float scaleDistance = baseDistance * Mathf.Clamp(GetScale(), .2f, 1f);

        float delta = Mathf.Abs(distance - scaleDistance) * 0.1f;

        // pull closer
        if (distance - scaleDistance > 0.01f)
        {
            curVel += delta * delta * direction;
        }

        // push away
        if (scaleDistance - distance > 0.01f)
        {
            curVel -= delta * delta * direction;
        }
    }
    #endregion



    #region Status

    private float GetScale()
    {
        return Mathf.Log(neighbours.Count * 0.5f + 1, 2);
    }

    private void SetStatus()
    {
        if (!spriteRenderer)
        {
            return;
        }

        Color color = NodeColor.colors[status % NodeColor.colors.Length];
        color.r /= 255.0f;
        color.g /= 255.0f;
        color.b /= 255.0f;
        spriteRenderer.color = color;
    }


    private void SetSize()
    {
        float scale = Mathf.Clamp(GetScale(), 0.5f, 3.0f);

        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void ChangeStatus()
    {
        status = (status + 1) % nState;

        SetStatus();
    }

    internal void AddNeighbour(GameObject node)
    {
        Node n = node.GetComponent<Node>();

        if (n != null)
        {
            this.neighbours.Add(n);
            n.neighbours.Add(this);
        }
    }
    #endregion



    #region Draw Graph

    private void CreateShadow()
    {
        if (shadowPrefab == null) return;

        Vector3 pos = transform.position;

        pos.x += .4f;
        pos.y -= .4f;
        pos.z += .1f;

        GameObject go = Instantiate(shadowPrefab, pos, transform.rotation);

        go.transform.SetParent(transform);
    }



    private void RemoveEdge()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.name == "Line") Destroy(child.gameObject);
        }
    }

    private void DrawEdge()
    {
        foreach (Node n in neighbours)
        {
            if (n.status == status)
            {
                DrawLine(transform.position, n.transform.position, spriteRenderer.color);
            }
            else
            {
                Color color = NodeColor.colors[2] / 255.0f;

                color.a = .8f;

                DrawLine(transform.position, n.transform.position, color, .05f, true);
            }
        }
    }

    private void DrawLine(Vector3 p1, Vector3 p2, Color color, float width = .05f, bool dash = false)
    {
        GameObject line = new GameObject();

        line.name = "Line";

        line.transform.SetParent(transform);

        LineRenderer lr = line.GetComponent<LineRenderer>();
        if (lr == null)
        {
            lr = line.AddComponent<LineRenderer>();
        }
        lr.material = new Material(Shader.Find("Hidden/Internal-Colored"));

        lr.startColor = color;
        lr.endColor = color;

        lr.startWidth = width;
        lr.endWidth = width;

        Vector3 c1 = new Vector3(p1.x, p1.y, p1.z + 1);
        Vector3 c2 = new Vector3(p2.x, p2.y, p2.z + 1);

        if (!dash)
        {
            lr.SetPosition(0, c1);
            lr.SetPosition(1, c2);
        }
        else
        {
            float leng = Vector3.Distance(c1, c2);
            Vector3 direction = Vector3.Normalize(c1 - c2);
            int n = Mathf.RoundToInt(leng * 5.0f);

            lr.positionCount = 2 * n;

            for (int i = 0; i < n; i++)
            {
                Vector3 pivot = Vector3.Lerp(c1, c2, leng / (float)n * i);

                lr.SetPosition(i * 2, pivot);
                lr.SetPosition(i * 2 + 1, pivot + direction * .1f);
            }
        }
    }
    #endregion



    #region Click

    private Vector3 screenPoint;
    private Vector3 offset;
    private bool dragging;
    private float mouseEnter = float.MaxValue;

    void OnMouseDown()
    {
        if (GameManager.instance.locked) return;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        mouseEnter = Time.time;
    }

    void OnMouseDrag()
    {
        if (GameManager.instance.locked) return;
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        transform.position = cursorPosition;
        if (Time.time - mouseEnter > .1f) dragging = true;
    }

    void OnMouseUp()
    {
        if (GameManager.instance.locked) return;
        if (dragging)
        {
            dragging = false;
            mouseEnter = float.MaxValue;
            return;
        }
        Step();

        SoundManager.instance.PlayClick();
    }
    #endregion
}
