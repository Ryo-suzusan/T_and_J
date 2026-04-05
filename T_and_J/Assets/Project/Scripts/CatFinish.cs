using UnityEngine;

public class CatFinish : MonoBehaviour
{
    [SerializeField]
    GameObject catCamera;

    private bool wasDestroyed = false;

    public void beCollected()
    {
        if (!wasDestroyed)
        {
            Destroy(gameObject);
            Destroy(catCamera);
            wasDestroyed = true;
        } 
    }
}
