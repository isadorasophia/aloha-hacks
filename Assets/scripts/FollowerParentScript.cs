using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerParentScript : MonoBehaviour
{
   Collider m_pCollider;

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }

    float TotalScale( Transform t )
    {
        if( t.parent == null )
        {
            return t.localScale.x;
        }
        else
        {
            float fParentsScale = TotalScale(t.parent);
            return fParentsScale * t.localScale.x;
        }
    }

    public FollowerScript GenerateFollower( PhysicMaterial pHandMaterial )
    {
        m_pCollider = GetComponent<Collider>();

        GameObject pNewFollower = new GameObject(name + "_F");
        pNewFollower.layer = 8;

        float fTotalScale = TotalScale(this.transform);
        pNewFollower.transform.localScale = new Vector3(fTotalScale, fTotalScale, fTotalScale);

        // can't have the follower be a child of anything, or physics won't work
        // pNewFollower.transform.parent = transform.parent;

        Rigidbody pNewRigidBody = pNewFollower.AddComponent<Rigidbody>();
        pNewRigidBody.position = transform.position;
        pNewRigidBody.rotation = transform.rotation;
        pNewRigidBody.centerOfMass = transform.position;
        pNewRigidBody.constraints = RigidbodyConstraints.None;
        pNewRigidBody.detectCollisions = true;
        pNewRigidBody.useGravity = false;
        pNewRigidBody.angularDrag = 0;
        pNewRigidBody.drag = 0;
        pNewRigidBody.detectCollisions = true;
        pNewRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        pNewRigidBody.mass = 100;
        // Kinematic makes it so when physics are applied, the hand itself is not affected. But it does cause a reaction on the other object.
        pNewRigidBody.isKinematic = true;

        // we need to copy the collider to the follower object, then delete the original collider.
        // the new collider also needs to be at the right scale
        //
        if (m_pCollider.GetType() == typeof(BoxCollider))
        {
            CopyComponent<BoxCollider>(m_pCollider as BoxCollider, pNewFollower);
            BoxCollider pBoxCollider = pNewFollower.GetComponent<BoxCollider>();
            Vector3 pColliderSize = pBoxCollider.size;
            pBoxCollider.size *= fTotalScale;
            pBoxCollider.material = pHandMaterial;
        }
        else if (m_pCollider.GetType() == typeof(CapsuleCollider))
        {
            CopyComponent<CapsuleCollider>(m_pCollider as CapsuleCollider, pNewFollower);
            CapsuleCollider pCapsuleCollider = pNewFollower.GetComponent<CapsuleCollider>();
            pCapsuleCollider.radius *= fTotalScale;
            pCapsuleCollider.height *= fTotalScale;
            pCapsuleCollider.contactOffset *= fTotalScale;
            pCapsuleCollider.material = pHandMaterial;
        }
        else if (m_pCollider.GetType() == typeof(SphereCollider))
        {
            CopyComponent<SphereCollider>(m_pCollider as SphereCollider, pNewFollower);
            SphereCollider pSphereCollider = pNewFollower.GetComponent<SphereCollider>();
            pSphereCollider.radius *= fTotalScale;
            pSphereCollider.contactOffset *= fTotalScale;
            pSphereCollider.material = pHandMaterial;
        }


        Destroy(m_pCollider);
        m_pCollider = null;

        pNewFollower.AddComponent<FollowerScript>();

        FollowerScript pScript = pNewFollower.GetComponent<FollowerScript>();
        pScript.SetWhoToFollow(this.transform);

        return pScript;
    }

}
