using UnityEngine;

public class Test : MonoBehaviour
{

    [SerializeField]
    Vector2 start;
    [SerializeField]
    Vector2 end;

    public int currentIndexX;
    public int currentIndexY;

    public Vector2 X;
    public Vector2 Y;

    private void OnDrawGizmos()
    {

        for(int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                Gizmos.DrawCube(new Vector3(x, 0, y), Vector3.one * 0.99f);
            }
        }

        Gizmos.color = Color.red;

        Gizmos.DrawLine(new Vector3(start.x, 2 , start.y), new Vector3(end.x, 2, end.y));

        float a = (end.y - start.y) / (end.x - start.x);
        float b = start.y - a * start.x;

        Vector2 dir = end - start;
        Vector2 current = new Vector2(start.x, start.y);

        float signX = dir.x > 0 ? 0.5f : -0.5f;
        float signY = dir.y > 0 ? 0.5f : -0.5f;

        currentIndexX = Mathf.RoundToInt(current.x); // x/y index of cell
        currentIndexY = Mathf.RoundToInt(current.y);

        Vector2Int endIndex = new Vector2Int(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y));


        for (int i = 0; i < 10; i++)
        {

            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(currentIndexX, 1, currentIndexY), Vector3.one * 0.2f);

            if (currentIndexX == endIndex.x && currentIndexY == endIndex.y)
                break;

            float nextY = a * (currentIndexX + signX) + b;
            float nextX = ((currentIndexY + signY) - b) / a;

            Gizmos.color = Color.blue;
            Y = new Vector2(currentIndexX + signX, nextY);
            //Gizmos.DrawWireSphere(new Vector3(Y.x, 0, Y.y), 0.2f);

            Gizmos.color = Color.green;
            X = new Vector2(nextX, currentIndexY + signY);
            //Gizmos.DrawWireSphere(new Vector3(X.x, 0, X.y), 0.2f);

            if (Vector2.SqrMagnitude(Y - current) < Vector2.SqrMagnitude(X - current))
            {
                Gizmos.color = Color.blue;
                current = Y;
                currentIndexX = Mathf.RoundToInt(current.x + signX);
                Gizmos.DrawWireSphere(new Vector3(current.x, 0, current.y), 0.2f);
            }
            else
            {
                Gizmos.color = Color.green;
                current = X;
                currentIndexY = Mathf.RoundToInt(current.y + signY);
                Gizmos.DrawWireSphere(new Vector3(current.x, 0, current.y), 0.2f);
            }

        }
    }
}
