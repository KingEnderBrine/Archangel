using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Archangel
{
    public class PreviewHitBox : HitBox
    {
        /*private static Material lineMaterial;
        private static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            }
        }

        public void OnEnable()
        {
            SceneCamera.onSceneCameraPostRender += SceneCameraPostRender;
        }

        private void OnDisable()
        {
            SceneCamera.onSceneCameraPostRender -= SceneCameraPostRender;
        }

        private void SceneCameraPostRender(SceneCamera sceneCamera)
        {
            CreateLineMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(transform.localToWorldMatrix);


            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            GL.Vertex3(0.5F, 0.5F, 0.5F);
            GL.Vertex3(-0.5F, 0.5F, 0.5F);

            GL.Vertex3(-0.5F, 0.5F, 0.5F);
            GL.Vertex3(-0.5F, -0.5F, 0.5F);

            GL.Vertex3(-0.5F, -0.5F, 0.5F);
            GL.Vertex3(0.5F, -0.5F, 0.5F);

            GL.Vertex3(0.5F, -0.5F, 0.5F);
            GL.Vertex3(0.5F, 0.5F, 0.5F);


            GL.Vertex3(0.5F, 0.5F, -0.5F);
            GL.Vertex3(-0.5F, 0.5F, -0.5F);

            GL.Vertex3(-0.5F, 0.5F, -0.5F);
            GL.Vertex3(-0.5F, -0.5F, -0.5F);

            GL.Vertex3(-0.5F, -0.5F, -0.5F);
            GL.Vertex3(0.5F, -0.5F, -0.5F);

            GL.Vertex3(0.5F, -0.5F, -0.5F);
            GL.Vertex3(0.5F, 0.5F, -0.5F);

            GL.Vertex3(0.5F, 0.5F, -0.5F);
            GL.Vertex3(0.5F, 0.5F, 0.5F);

            GL.Vertex3(-0.5F, 0.5F, -0.5F);
            GL.Vertex3(-0.5F, 0.5F, 0.5F);

            GL.Vertex3(-0.5F, -0.5F, -0.5F);
            GL.Vertex3(-0.5F, -0.5F, 0.5F);
            
            GL.Vertex3(0.5F, -0.5F, -0.5F);
            GL.Vertex3(0.5F, -0.5F, 0.5F);
            GL.End();

            GL.PopMatrix();
        }*/

        private void OnDrawGizmosSelected()
        {
            var color = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
            Gizmos.color = color;
        }
    }
}