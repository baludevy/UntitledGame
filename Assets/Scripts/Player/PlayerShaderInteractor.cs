using UnityEngine;
 
public class PlayerShaderInteractor : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalVector("_PositionMoving", transform.position);
    }
}