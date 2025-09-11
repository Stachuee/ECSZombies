using UnityEngine;

public class ZombiesData
{
    [Header("MaxHealth")]
    public float Health;

    public static ZombiesData Default()
    {
        return new ZombiesData()
        {
            Health = 100
        };
    }
}




