using UnityEngine;

namespace Background.SplinePath
{
    [RequireComponent(typeof(MeshCollider), typeof(LineRenderer))]
    public class SplineCollider2D : MonoBehaviour
    {
        private MeshCollider myMeshCollider;
        private LineRenderer myLineRender;

        public void CreateCollider()
        {
            if (!myLineRender) myLineRender = GetComponent<LineRenderer>();

            if (!myMeshCollider) myMeshCollider = GetComponent<MeshCollider>();

            Mesh mesh = new Mesh();
            myLineRender.BakeMesh(mesh,true);
            myMeshCollider.sharedMesh = mesh;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(myMeshCollider);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}