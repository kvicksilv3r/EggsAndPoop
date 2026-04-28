using UnityEngine;

namespace IconsCreationTool.Editor.Utility.Extensions
{
    public static class GameObjectExtensions
    {
        public static Bounds GetOrthographicBounds(this GameObject gameObject, Camera camera)
        {
            Vector3 minScreenPosition = Vector3.positiveInfinity;
            Vector3 maxScreenPosition = Vector3.negativeInfinity;

            foreach (MeshFilter meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
            {
                if (!meshFilter.sharedMesh) continue;
                AccumulateVertices(meshFilter.sharedMesh.vertices, meshFilter.transform, camera, ref minScreenPosition, ref maxScreenPosition);
            }

            foreach (SkinnedMeshRenderer smr in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (!smr.sharedMesh) continue;
                AccumulateVertices(smr.sharedMesh.vertices, smr.transform, camera, ref minScreenPosition, ref maxScreenPosition);
            }

            Bounds bounds = new Bounds();
            bounds.SetMinMax(
                camera.ScreenToWorldPoint(minScreenPosition),
                camera.ScreenToWorldPoint(maxScreenPosition));
            return bounds;
        }

        private static void AccumulateVertices(Vector3[] vertices, Transform transform, Camera camera,
            ref Vector3 minScreenPosition, ref Vector3 maxScreenPosition)
        {
            foreach (Vector3 vertex in vertices)
            {
                Vector3 screenPosition = camera.WorldToScreenPoint(transform.TransformPoint(vertex));
                for (int i = 0; i < 3; i++)
                {
                    minScreenPosition[i] = Mathf.Min(minScreenPosition[i], screenPosition[i]);
                    maxScreenPosition[i] = Mathf.Max(maxScreenPosition[i], screenPosition[i]);
                }
            }
        }


        public static bool HasVisibleMesh(this GameObject gameObject)
        {
            foreach (MeshFilter meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
            {
                if (meshFilter && meshFilter.sharedMesh && meshFilter.GetComponent<MeshRenderer>())
                    return true;
            }

            foreach (SkinnedMeshRenderer smr in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (smr && smr.sharedMesh)
                    return true;
            }

            return false;
        }
    }
}