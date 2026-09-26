using UnityEngine;

public class HunterAnimationEvent : MonoBehaviour
{
    public HunterControl3d hunter;

    public void FireProjectile()
    {
        hunter.FireProjectile();
    }
}