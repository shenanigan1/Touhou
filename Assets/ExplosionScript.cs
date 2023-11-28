using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    public void EndAnimation()
    {
        PoolController.Instance.Suppr(gameObject, Type.Explosion);
    }
}
