using UnityEngine;

public static class QuaternionExtensions
{
    // To get the local right vector from just a rotation.
    public static Vector3 GetRight (this Quaternion rotation)
    {
        return new Vector3(1 - 2 * (rotation.y * rotation.y + rotation.z * rotation.z),
                    2 * (rotation.x * rotation.y + rotation.w * rotation.z),
                    2 * (rotation.x * rotation.z - rotation.w * rotation.y));
    }


    // To get the local up vector from just a rotation.
    public static Vector3 GetUp(this Quaternion rotation)
    {
        return new Vector3(2 * (rotation.x * rotation.y - rotation.w * rotation.z),
                    1 - 2 * (rotation.x * rotation.x + rotation.z * rotation.z),
                    2 * (rotation.y * rotation.z + rotation.w * rotation.x));
    }


    // To get the local forward vector from just a rotation.
    public static Vector3 GetForward(this Quaternion rotation)
    {
        return new Vector3(2f * (rotation.x * rotation.z + rotation.w * rotation.y),
                    2f * (rotation.y * rotation.x - rotation.w * rotation.x),
                    1f - 2f * (rotation.x * rotation.x + rotation.y * rotation.y));
    }
}
