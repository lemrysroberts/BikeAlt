using UnityEngine;
using System.Linq;
using System.Collections;

public class Steering : MonoBehaviour
{
    public int NumIncrements = 10;
    public float AngleRange = 45.0f;
    public float RaycastRange = 1.0f;

    //public int HysteresisFrames = 3;

    public float UpdateSteering(float angleToGlobalTarget)
    {
        float angleDelta = (AngleRange * 2.0f) / (float)NumIncrements;

        float startAngle = -AngleRange;
        float endAngle = AngleRange;

        // Debug.Log("Angle: " + angleToGlobalTarget);

        bool previousBlocked = false;

        float[] scores = new float[NumIncrements];

        for (int increment = 0; increment < NumIncrements; ++increment)
        {
            float angle = startAngle + (angleDelta * (float)increment);

            Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));
            Vector3 direction = rotation * Vector3.forward;

            Vector3 targetVec = Quaternion.Euler(new Vector3(0.0f, angleToGlobalTarget, 0.0f)) * Vector3.forward;

            scores[increment] = 1.0f - Mathf.Pow(Vector3.Dot(targetVec, direction), 4.0f);

            if (previousBlocked)
            {
                scores[increment] += 0.3f;
                previousBlocked = false;
            }

            // Check out of bounds:
            // Get offsetX at transform.position.y + direction.y and check to see if further out

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, direction, out hitInfo, RaycastRange))
            {
                // Increase the heat from a hit proportional to distance of the hit, so closer obstacles are more dangerous
                scores[increment] = 1.0f - Mathf.Pow(hitInfo.distance / RaycastRange, 2.0f);
                previousBlocked = true;

                if (increment > 0)
                {
                    scores[increment - 1] += 0.3f;
                }
            }


        }

        const int kernelSize = 5;
        const int halfKernelSize = kernelSize / 2;

        float[] blurredScores = new float[NumIncrements];

        // Blur score values
        for (int increment = 0; increment < NumIncrements; ++increment)
        {
            float total = 0.0f;
            float contributions = 0;

            for (int otherIncrement = Mathf.Max(increment - halfKernelSize, 0); otherIncrement < Mathf.Min(increment + halfKernelSize, NumIncrements); ++otherIncrement)
            {
                total += scores[otherIncrement];
                contributions++;
            }

            blurredScores[increment] = total / (float)contributions;            
        }

        int maxIndex = -1;
        float minScore = 100.0f;
        for (int increment = 0; increment < NumIncrements; ++increment)
        {
            float angle = startAngle + (angleDelta * (float)increment);

            Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, angle, 0.0f));
            Vector3 direction = rotation * Vector3.forward;

            HSBColor lineColor = HSBColor.Lerp(new HSBColor(Color.green), new HSBColor(Color.red), blurredScores[increment]);

            Debug.DrawLine(transform.position, transform.position + direction * RaycastRange, lineColor.ToColor());

            if(blurredScores[increment] < minScore)
            {
                minScore = blurredScores[increment];
                maxIndex = increment;
            }
        }

        float winnerAngle = startAngle + (angleDelta * (float)maxIndex);

        Quaternion winnerRotation = Quaternion.Euler(new Vector3(0.0f, winnerAngle, 0.0f));
        Vector3 winnerDirection = winnerRotation * Vector3.forward;
        Debug.DrawLine(transform.position, transform.position + winnerDirection * 1.3f, Color.magenta);

        return winnerDirection.x;
    }
}
