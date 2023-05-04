using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Scripts
{
    class Slicer
    {
        /// <summary>
        /// Slice the object by the plane 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="objectToCut"></param>
        /// <returns></returns>
        public static GameObject[] Slice(Plane plane, GameObject objectToCut)
        {
            //Get the current mesh and its verts and tris
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();
            if (!sliceable)
            {
                sliceable = objectToCut.GetComponentInParent<Sliceable>();
            }

            if (sliceable == null)
            {
                throw new NotSupportedException(
                    "Cannot slice non sliceable object, add the sliceable script to the object or inherit from sliceable to support slicing");
            }


            Mesh mesh = sliceable.mesh;
            if (!mesh)
            {
                mesh = objectToCut.GetComponent<MeshFilter>().mesh;
                var a = mesh.GetSubMesh(0);
            }

            //Create left and right slice of hollow object
            SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.IsSolid,
                sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);

            GameObject positiveObject = CreateMeshGameObject(objectToCut);
            positiveObject.name = string.Format("{0}_positive", objectToCut.name);

            GameObject negativeObject = CreateMeshGameObject(objectToCut);
            negativeObject.name = string.Format("{0}_negative", objectToCut.name);

            var positiveSideMeshData = slicesMeta.PositiveSideMesh;
            var negativeSideMeshData = slicesMeta.NegativeSideMesh;

            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

            SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, sliceable.UseGravity);
            SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, sliceable.UseGravity);

            GameObject.Destroy(objectToCut);
            
            return new GameObject[] {positiveObject, negativeObject};
        }

        public static GameObject[] Slice(Plane plane, Sliceable sliceable)
        {
            //Get the current mesh and its verts and tris
            Mesh mesh = sliceable.mesh;

            //Create left and right slice of hollow object
            SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.IsSolid,
                sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);
            if (!slicesMeta.IsSliced())
            {
                return new GameObject[] { };
            }
            
            GameObject positiveObject = CreateSliceableCopy(sliceable, string.Format("{0}_p", sliceable.name));
            GameObject negativeObject = CreateSliceableCopy(sliceable, string.Format("{0}_n", sliceable.name));

            Sliceable positiveSliceable = positiveObject.GetComponent<Sliceable>();
            Sliceable negativeSliceable = negativeObject.GetComponent<Sliceable>();

            positiveSliceable.Init(slicesMeta.PositiveSideMesh,sliceable.Rigidbody,1f);
            negativeSliceable.Init(slicesMeta.NegativeSideMesh,sliceable.Rigidbody,-.8f);
            
            positiveSliceable.OnSlice_Spawn();
            sliceable.OnSlice_Destroy();
            GameObject.Destroy(sliceable.gameObject);

            return new GameObject[] {positiveObject, negativeObject};
        }

        /// <summary>
        /// Creates the default mesh game object.
        /// </summary>
        /// <param name="originalObject">The original object.</param>
        /// <returns></returns>
        private static GameObject CreateMeshGameObject(GameObject originalObject)
        {
            var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

            GameObject meshGameObject = new GameObject();
            Sliceable originalSliceable = originalObject.GetComponent<Sliceable>();

            meshGameObject.AddComponent<MeshFilter>();
            meshGameObject.AddComponent<MeshRenderer>();
            Sliceable sliceable = meshGameObject.AddComponent<Sliceable>();

            sliceable.IsSolid = originalSliceable.IsSolid;
            sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            sliceable.UseGravity = originalSliceable.UseGravity;

            meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

            meshGameObject.transform.localScale = originalObject.transform.localScale;
            meshGameObject.transform.rotation = originalObject.transform.rotation;
            meshGameObject.transform.position = originalObject.transform.position;

            meshGameObject.tag = originalObject.tag;

            return meshGameObject;
        }

        // private static GameObject CreateMeshGameObject(Sliceable originalSliceable)
        // {
        //     var originalMaterial = originalSliceable.materials;
        //
        //     GameObject meshGameObject = new GameObject();
        //
        //     MeshFilter mf = meshGameObject.AddComponent<MeshFilter>();
        //     MeshRenderer mr = meshGameObject.AddComponent<MeshRenderer>();
        //     Sliceable sliceable = meshGameObject.AddComponent<Sliceable>();
        //     sliceable.Init(mr, mf);
        //
        //     sliceable.IsSolid = originalSliceable.IsSolid;
        //     sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
        //     sliceable.UseGravity = originalSliceable.UseGravity;
        //
        //     mr.materials = originalMaterial;
        //
        //     meshGameObject.transform.localScale = originalSliceable.transform.localScale;
        //     meshGameObject.transform.rotation = originalSliceable.transform.rotation;
        //     meshGameObject.transform.position = originalSliceable.transform.position;
        //
        //     meshGameObject.tag = originalSliceable.tag;
        //
        //     return meshGameObject;
        // }

        private static GameObject CreateSliceableCopy(Sliceable originalSliceable, string prefix)
        {
            var originalMaterial = originalSliceable.materials;

            GameObject meshGameObject = GameObject.Instantiate(originalSliceable.gameObject,
                originalSliceable.transform.position, originalSliceable.transform.rotation);

            meshGameObject.name = prefix;
            Sliceable sliceable = meshGameObject.GetComponent<Sliceable>();
            sliceable.SelfInit();
            // sliceable.Init(mr, mf);
            //
            // sliceable.IsSolid = originalSliceable.IsSolid;
            // sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            // sliceable.UseGravity = originalSliceable.UseGravity;
            //
            // mr.materials = originalMaterial;

            // meshGameObject.transform.localScale = originalSliceable.transform.localScale;

            // meshGameObject.tag = originalSliceable.tag;

            return meshGameObject;
        }


        /// <summary>
        /// Add mesh collider and rigid body to game object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="mesh"></param>
        private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;

            var rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = useGravity;
        }
        

    }
}