using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 6f;

    void Update()
    {
        float x = 0f;
        float z = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) x = 1f;
            if (Keyboard.current.aKey.isPressed) x = -1f;
            if (Keyboard.current.wKey.isPressed) z = 1f;
            if (Keyboard.current.sKey.isPressed) z = -1f;
        }

        transform.Translate(new Vector3(x, 0f, z) * velocidad * Time.deltaTime);
    }
}
