using UnityEngine;

public interface IGravityChanger
{
    Vector3 GetGravityDirection(int rowIndex); // return Quaternion instead?? And with it rotate...
}

public class RoundGravityChanger : IGravityChanger
{
    public Vector3 GetGravityDirection(int rowIndex)
    {
        // if row > 10 => base class ?

        if ((rowIndex / 45) % 4 == 0)
            return Quaternion.Euler((rowIndex % 45) / 2f, 0f, 0f) * Vector3.down;
        if ((rowIndex / 45) % 4 == 1)
            return Quaternion.Euler((45 - (rowIndex % 45)) / 2f, 0f, 0f) * Vector3.down;
        if ((rowIndex / 45) % 4 == 2)
            return Quaternion.Euler(-(rowIndex % 45) / 2f, 0f, 0f) * Vector3.down;

        return Quaternion.Euler((-45 + (rowIndex % 45)) / 2f, 0f, 0f) * Vector3.down;

        /*if((rowIndex / 45) % 4 == 0)
            return Quaternion.Euler(0f, 0f, (rowIndex % 45) / 2f) * Vector3.down;
        if ((rowIndex / 45) % 4 == 1)
            return Quaternion.Euler(0f, 0f, (45 - (rowIndex % 45)) / 2f) * Vector3.down;
        if ((rowIndex / 45) % 4 == 2)
            return Quaternion.Euler(0f, 0f, -(rowIndex % 45) / 2f) * Vector3.down;

        return Quaternion.Euler(0f, 0f, (-45 + (rowIndex % 45)) / 2f) * Vector3.down;*/
    }
}
