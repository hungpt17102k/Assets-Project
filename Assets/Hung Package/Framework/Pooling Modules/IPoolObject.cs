using UnityEngine;

/* Suguestion
    This class should attach with all pool object
*/
public interface IPoolObject
{
    void OnObjectReuse();

    void OnDestroyObject();
}

/* Example:
public class Bullet : MonoBehaviour, IPoolObject
{
    public void OnObjectReuse()
    {
        // Action you want to do when this bullet enable
    }

    public void OnDestroyObject()
    {
        // Disable this bullet, not destroy
        gameObject.SetActive(false);
    }

*/