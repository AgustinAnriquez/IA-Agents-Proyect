using UnityEngine;
using UnityEngine.InputSystem; // Obligatorio para que Unity no se queje

public class PlayerMovement : MonoBehaviour
{
    public float velocidad = 6f;

    void Update()
    {
        float x = 0f;
        float z = 0f;

        // Leemos las teclas WASD directamente desde el teclado activo
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed) x = 1f;
            if (Keyboard.current.aKey.isPressed) x = -1f;
            if (Keyboard.current.wKey.isPressed) z = 1f;
            if (Keyboard.current.sKey.isPressed) z = -1f;
        }

        // Mueve el objeto ignorando las colisiones (ideal para testear rápido)
        transform.Translate(new Vector3(x, 0f, z) * velocidad * Time.deltaTime);
    }
}