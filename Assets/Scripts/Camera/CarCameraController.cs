using UnityEngine;

class CarCameraController : CameraController
{
    protected override void Zoom()
    {
        maxPlayerDistance = 20.0f;

        foreach (var target in targets)
        {
            var distance = Vector3.Distance(averagePlayerPosition, target.position);

            if (distance > maxPlayerDistance)
            {
                maxPlayerDistance = distance;
            }
        }

        offset = (new Vector3(0, 1.2f, -1.5f) * maxPlayerDistance);
    }

    /*protected override void FindAveragePosition()
    {
        foreach (var target in targets)
        {
            if(target.position.z > averagePlayerPosition.z)
            {
                averagePlayerPosition = target.position;
            }
        }
    }*/
}
