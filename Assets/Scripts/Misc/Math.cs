using UnityEngine;
using System.Collections.Generic;

public class Math : MonoBehaviour
{
    public static float GetAngle(Vector3 pointA, Vector3 pointB)
    {
        float AngleToTarget = Vector3.Angle(pointA - pointB, new Vector3(1.00f, 0.00f, 0.00f));
        var Cross = Vector3.Cross(pointA - pointB, new Vector3(1.00f, 0.00f, 0.00f));
        if (Cross.y <= 0)
            AngleToTarget = -AngleToTarget;
        return AngleToTarget;
    }
    public static float GetAngleRaw(Vector3 pointA, Vector3 pointB)
    {
        float val = -(Mathf.Atan2(pointA.x - pointB.x, pointA.z - pointB.z) * Mathf.Rad2Deg) - 90.00f;
        if (val < -180.00f) { val += 360.00f; }
        return val;
    }
    public static float GetDistance2D(Vector3 pointA, Vector3 pointB)
    {
        return Vector3.Distance(new Vector3(pointA.x, 0.00f, pointA.z), new Vector3(pointB.x, 0.00f, pointB.z));
    }
    public static Vector3 RotateVector(Vector3 data, Vector3 rotationAxis, float angle)
    {
        return Quaternion.AngleAxis(angle, rotationAxis) * data;
    }
    public static Vector3 PolarVector2D(Vector3 data, float angle, float distance)
    {
        return new Vector3(data.x + distance * Mathf.Cos(angle * Mathf.Deg2Rad), data.y, data.z + distance * Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    public static float Round(float value, int decimalPlaces = 0)
    {
        return Mathf.Round(value * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
    }
    public static float UnityRotationToGeneralRotation(float eulerAnglesY)
    {
        return (eulerAnglesY > 180f ? 360f - eulerAnglesY : -eulerAnglesY);
    }
    public static float GetAngleDifference(float a, float b)
    {
        float dif = b - a;
        if (dif > 180.00f) { dif -= 360.00f; }
        else if (dif < -180.00f) { dif += 360.00f; }
        return dif;
    }
}

public static class Utility
{
    public static bool IsThisPlayer(GameObject suspect)
    {
        return suspect.tag == "Player";
    }

    // Translate mouse position into a point on the plane
    public static Vector3 GetMouseWorldPosition(Vector3 planePoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance = 0;
        Plane hPlane = new Plane(Vector3.up, planePoint);
        if (hPlane.Raycast(ray, out distance))
            return ray.GetPoint(distance);
        else
            return new Vector3(0, 0, 0);
    }
    
    public static Vector3 GetGroundPosition(Vector3 fromPosition) {
        RaycastHit hit;
        const int walkableLayerMask = 1 << 9;
        var ray = new Ray(fromPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, 1000.00f, walkableLayerMask)) {
            return hit.point;
        }

        return Vector3.zero;
    }
    
    public static float GetDistanceToGround(Vector3 fromPosition) {
        RaycastHit hit;
        const int walkableLayerMask = 1 << 9;
        var ray = new Ray(fromPosition, Vector3.down);

        if (Physics.Raycast(ray, out hit, 1000.00f, walkableLayerMask)) {
            return hit.distance;
        }

        return 1000.00f;
    }

    // Returns the distance to the first obstacle from position to a specified direction
    public static float GetDistanceToObstacle(Vector3 fromPosition, Vector3 direction)
    {
        Ray ray = new Ray(fromPosition, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        float closestDist = 1000.00f;
        RaycastHit closestHit = new RaycastHit();
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.tag == "Geometry" && hit.distance < closestDist)
            {
                closestHit = hit;
                closestDist = hit.distance;
            }
        }
        if (closestDist < 1000.00f)
        {
            return closestDist;
        }
        else
            return 1000.00f;
    }

    // Checks if the target is visible from the start point
    public static bool IsTargetObstructed(Vector3 start, Vector3 target)
    {
        // Raycast to check for obstacles
        Ray ray = new Ray(start, target - start);
        RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(start, target));
        foreach (RaycastHit hit in hits)
        {
            // If a static object is hit, return
            if (hit.transform.tag == "Geometry")
            {
                return true;
            }
        }
        return false;
    }

    public static List<string> ColorStringList = new List<string>()
    {
        "aqua",   "black",  "blue",      "brown",  "cyan",    "darkblue", "fuchsia", "green",
        "gray",   "grey",   "lightblue", "lime",   "magenta", "maroon",   "navy",    "olive",
        "orange", "purple", "red",       "silver", "teal",    "white",    "yellow"
    };
    public static string ColorStringToHex(string input)
    {
        switch (input)
        {
            case "aqua":        return "#00ffffff";
            case "black":       return "#000000ff";
            case "blue":        return "#0000ffff";
            case "brown":       return "#a52a2aff";
            case "cyan":        return "#00ffffff";
            case "darkblue":    return "#0000a0ff";
            case "fuchsia":     return "#ff00ffff";
            case "green":       return "#008000ff";
            case "gray":        return "#808080ff";
            case "grey":        return "#808080ff";
            case "lightblue":   return "#add8e6ff";
            case "lime":        return "#00ff00ff";
            case "magenta":     return "#ff00ffff";
            case "maroon":      return "#800000ff";
            case "navy":        return "#000080ff";
            case "olive":       return "#808000ff";
            case "orange":      return "#ffa500ff";
            case "purple":      return "#800080ff";
            case "red":         return "#ff0000ff";
            case "silver":      return "#c0c0c0ff";
            case "teal":        return "#008080ff";
            case "white":       return "#ffffffff";
            case "yellow":      return "#ffff00ff";
        }
        return input;
    }

    // Converts all colors from "yellow" to "#ffff00ff"
    public static string ConvertColorStrings(string input)
    {
        string output = input;

        for (int i = 0; i < ColorStringList.Count; i++)
        {
            output = output.Replace("<color=" + ColorStringList[i], "<color=" + ColorStringToHex(ColorStringList[i]));
        }
        return output;
    }

    // Daniel Brauer, big fucking thanks :D
    public static float FirstOrderInterceptTime(float projectileSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity)
    {
        float velocitySquared = targetRelativeVelocity.sqrMagnitude;
        if (velocitySquared < 0.001f)
            return 0f;

        float a = velocitySquared - projectileSpeed * projectileSpeed;

        //handle similar velocities
        if (Mathf.Abs(a) < 0.001f)
        {
            float t = -targetRelativePosition.sqrMagnitude /
            (
                2f * Vector3.Dot
                (
                    targetRelativeVelocity,
                    targetRelativePosition
                )
            );
            return Mathf.Max(t, 0f); //don't shoot back in time
        }

        float b = 2f * Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
        float c = targetRelativePosition.sqrMagnitude;
        float determinant = b * b - 4f * a * c;

        if (determinant > 0f)
        { //determinant > 0; two intercept paths (most common)
            float t1 = (-b + Mathf.Sqrt(determinant)) / (2f * a),
                    t2 = (-b - Mathf.Sqrt(determinant)) / (2f * a);
            if (t1 > 0f)
            {
                if (t2 > 0f)
                    return Mathf.Min(t1, t2); //both are positive
                else
                    return t1; //only t1 is positive
            }
            else
                return Mathf.Max(t2, 0f); //don't shoot back in time
        }
        else if (determinant < 0f) //determinant < 0; no intercept path
            return 0f;
        else //determinant = 0; one intercept path, pretty much never happens
            return Mathf.Max(-b / (2f * a), 0f); //don't shoot back in time
    }
}