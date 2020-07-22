//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;

using UnityEngine;


namespace zSpace.zView
{
    public class BoxMask : MonoBehaviour
    {
        //////////////////////////////////////////////////////////////////
        // Unity Monobehaviour Callbacks
        //////////////////////////////////////////////////////////////////

        void Awake()
        {
            this.CreateMesh();
        }


        //////////////////////////////////////////////////////////////////
        // Public API
        //////////////////////////////////////////////////////////////////

        public void SetSize(Vector3 size)
        {
            if (size != _size)
            {
                // Update the mesh vertices corresponding to the outer
                // extents of the box mask.
                Vector3 halfSize = size * 0.5f;
                Vector3[] vertices = _mesh.vertices;

                vertices[4]  = new Vector3(-halfSize.x,  halfSize.y,  0);
                vertices[5]  = new Vector3( halfSize.x,  halfSize.y,  0);
                vertices[6]  = new Vector3( halfSize.x, -halfSize.y,  0);
                vertices[7]  = new Vector3(-halfSize.x, -halfSize.y,  0);

                vertices[8]  = new Vector3(-halfSize.x,  halfSize.y, -size.z);
                vertices[9]  = new Vector3( halfSize.x,  halfSize.y, -size.z);
                vertices[10] = new Vector3( halfSize.x, -halfSize.y, -size.z);
                vertices[11] = new Vector3(-halfSize.x, -halfSize.y, -size.z);

                _mesh.vertices = vertices;

                // Cache the new size.
                _size = size;
            }
        }

        public void SetCutoutSize(Vector2 size)
        {
            if (size != _cutoutSize)
            {
                // Update the mesh vertices corresponding to the extents
                // of back face's cutout.
                Vector2 halfSize = size * 0.5f;
                Vector3[] vertices = _mesh.vertices;

                vertices[0] = new Vector3( halfSize.x,  halfSize.y,  0);
                vertices[1] = new Vector3( halfSize.x, -halfSize.y,  0);
                vertices[2] = new Vector3(-halfSize.x, -halfSize.y,  0);
                vertices[3] = new Vector3(-halfSize.x,  halfSize.y,  0);

                _mesh.vertices = vertices;

                // Cache the new cutout size.
                _cutoutSize = size;
            }
        }

        public void SetRenderQueue(int renderQueue)
        {
            if (_meshRenderer != null && _meshRenderer.material != null)
            {
                _meshRenderer.material.renderQueue = renderQueue;
            }
        }
        

        //////////////////////////////////////////////////////////////////
        // Private Methods
        //////////////////////////////////////////////////////////////////

        private void CreateMesh()
        {
            // Create the mesh.
            _mesh = new Mesh();
            _mesh.name = "BoxMask";
            _mesh.vertices = new Vector3[12];
            _mesh.triangles = new int[]
            {
                // Back Face Top:
                0, 4, 5,
                0, 5, 1,

                // Back Face Right:
                1, 5, 6,
                1, 6, 2,

                // Back Face Bottom:
                2, 6, 7,
                2, 7, 3,

                // Back Face Left:
                3, 7, 4,
                3, 4, 0,

                // Top Face:
                4, 8, 9,
                4, 9, 5,

                // Right Face:
                5, 9, 10,
                5, 10, 6,

                // Bottom Face:
                6, 10, 11,
                6, 11, 7,

                // Right Face:
                7, 11, 8,
                7, 8, 4,

                // Front Face:
                8, 10, 9,
                8, 11, 10,
            };

            // Attempt to find the depth mask shader.
            Shader depthMaskShader = Shader.Find("zSpace/zView/DepthMask");
            if (depthMaskShader == null)
            {
                Debug.LogError("Failed to find the zSpace/zView/DepthMask shader.");
            }

            // Create the mesh filter and update its mesh.
            _meshFilter = this.gameObject.AddComponent<MeshFilter>();
            _meshFilter.mesh = _mesh;

            // Create the mesh renderer and update its material.
            _meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
            if (depthMaskShader != null)
            {
                _meshRenderer.material = new Material(depthMaskShader);
            }
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private MeshFilter   _meshFilter   = null;
        private MeshRenderer _meshRenderer = null;
        private Mesh         _mesh         = null;
        private Vector3      _size         = Vector3.zero;
        private Vector2      _cutoutSize   = Vector2.zero;
    }
}