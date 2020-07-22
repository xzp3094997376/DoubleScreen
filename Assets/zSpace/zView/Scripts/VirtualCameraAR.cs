//////////////////////////////////////////////////////////////////////////
//
//  Copyright (C) 2007-2016 zSpace, Inc.  All Rights Reserved.
//
//////////////////////////////////////////////////////////////////////////

using System;

using UnityEngine;


namespace zSpace.zView
{
    public class VirtualCameraAR : VirtualCamera
    {
        //////////////////////////////////////////////////////////////////
        // Unity MonoBehaviour Callbacks
        //////////////////////////////////////////////////////////////////

        void Awake()
        {
            this.CreateCameras();
            this.CreateBoxMask();
            this.LoadResources();
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Material compositorMaterial = (_isTransparencyEnabled) ? _compositorMaterialRGBA : _compositorMaterialRGB;
            if (compositorMaterial == null)
            {
                Graphics.Blit(src, dest);
                return;
            }

            if (_isTransparencyEnabled)
            {
                compositorMaterial.SetTexture("_MaskDepthTexture", _maskDepthRenderTexture);
                compositorMaterial.SetTexture("_NonEnvironmentTexture", _nonEnvironmentRenderTexture);
            }
            else
            {
                compositorMaterial.SetTexture("_MaskDepthTexture", _maskDepthRenderTexture);
                compositorMaterial.SetTexture("_NonEnvironmentDepthTexture", _nonEnvironmentRenderTexture);
                compositorMaterial.SetColor("_MaskColor", MASK_COLOR);
            }

            Graphics.Blit(src, dest, compositorMaterial, 0);
        }


        //////////////////////////////////////////////////////////////////
        // Virtual Camera Overrides
        //////////////////////////////////////////////////////////////////

        public override void SetUp(ZView zView, IntPtr connection, ZView.ModeSetupPhase phase)
        {
            switch (phase)
            {
                case ZView.ModeSetupPhase.Initialization:
                    // Do nothing.
                    break;

                case ZView.ModeSetupPhase.Completion:
                    // Grab the image dimensions from the connection settings.
                    _imageWidth = zView.GetSettingUInt16(connection, ZView.SettingKey.ImageWidth);
                    _imageHeight = zView.GetSettingUInt16(connection, ZView.SettingKey.ImageHeight);

                    // Create the mask depth render texture (mask only).
                    // NOTE: This is used for both RGB or RGBA overlay.
                    _maskDepthRenderTexture = new RenderTexture((int)_imageWidth, (int)_imageHeight, 24, RenderTextureFormat.ARGB32);
                    _maskDepthRenderTexture.filterMode = FilterMode.Point;
                    _maskDepthRenderTexture.name = "MaskDepthRenderTexture";
                    _maskDepthRenderTexture.Create();

                    // Create the non-environment render texture (non-environment objects + mask).
                    // NOTE: For the RGB overlay, this is used to perform a depth render
                    //       of the non-environment objects (excluding the mask). For the RGBA overlay, 
                    //       this is used to render non-environment objects (including the mask depth).
                    _nonEnvironmentRenderTexture = new RenderTexture((int)_imageWidth, (int)_imageHeight, 24, RenderTextureFormat.ARGB32);
                    _nonEnvironmentRenderTexture.filterMode = FilterMode.Point;
                    _nonEnvironmentRenderTexture.name = "NonEnvironmentRenderTexture";
                    _nonEnvironmentRenderTexture.Create();

                    // Create the final composite render texture.
                    // NOTE: This is used for both RGB or RGBA overlay.
                    _finalRenderTexture = new RenderTexture((int)_imageWidth, (int)_imageHeight, 24, RenderTextureFormat.ARGB32);
                    _finalRenderTexture.filterMode = FilterMode.Point;
                    _finalRenderTexture.name = "CompositeRenderTexture";
                    _finalRenderTexture.Create();

                    // Cache the composite render texture's native texture pointer. Per Unity documentation,
                    // calling GetNativeTexturePtr() when using multi-threaded rendering will
                    // synchronize with the rendering thread (which is a slow operation). So, only
                    // call and cache once upon initialization.
                    _nativeTexturePtr = _finalRenderTexture.GetNativeTexturePtr();

                    break;

                default:
                    break;
            }
        }

        public override void TearDown()
        {
            // Reset the camera's target texture.
            _compositorCamera.targetTexture = null;

            // Reset the render texture's native texture pointer.
            _nativeTexturePtr = IntPtr.Zero;

            // Reset the image width and height;
            _imageWidth = 0;
            _imageHeight = 0;

            // Clean up the existing render textures.
            if (_maskDepthRenderTexture != null)
            {
                UnityEngine.Object.Destroy(_maskDepthRenderTexture);
                _maskDepthRenderTexture = null;
            }

            if (_nonEnvironmentRenderTexture != null)
            {
                UnityEngine.Object.Destroy(_nonEnvironmentRenderTexture);
                _nonEnvironmentRenderTexture = null;
            }

            if (_finalRenderTexture != null)
            {
                UnityEngine.Object.Destroy(_finalRenderTexture);
                _finalRenderTexture = null;
            }
        }

        public override void Render(ZView zView, IntPtr connection, IntPtr receivedFrame)
        {
            // Grab a reference to the CoreProxy instance.
            ZCoreProxy coreProxy = ZCoreProxy.Instance;
            if (coreProxy.CoreObject == null)
            {
                Debug.LogError("Failed to find instance of zSpace.Core.Core Monobehaviour.");
            }
            
            // Cache whether transparency is enabled.
            _isTransparencyEnabled = zView.ARModeEnableTransparency;

            // Grab the viewer scale.
            float viewerScale = coreProxy.ViewerScale;


            ///////////////////////////////
            // Camera Properties Update
            ///////////////////////////////

            // Cache the camera's culling mask and near/far clip planes so that they
            // can be restored after it renders the frame.
            int originalCullingMask = _compositorCamera.cullingMask;
            float originalNearClipPlane = _compositorCamera.nearClipPlane;
            float originalFarClipPlane = _compositorCamera.farClipPlane;

            // Grab the web cam's display space pose matrix and intrinsic values
            // from the frame data.
            Matrix4x4 cameraPoseMatrixInDisplaySpace = zView.GetFrameDataMatrix4x4(receivedFrame, ZView.FrameDataKey.CameraPose);
            float focalLength = zView.GetFrameDataFloat(receivedFrame, ZView.FrameDataKey.CameraFocalLength);
            float principalPointOffsetX = zView.GetFrameDataFloat(receivedFrame, ZView.FrameDataKey.CameraPrincipalPointOffsetX);
            float principalPointOffsetY = zView.GetFrameDataFloat(receivedFrame, ZView.FrameDataKey.CameraPrincipalPointOffsetY);
            float pixelAspectRatio = zView.GetFrameDataFloat(receivedFrame, ZView.FrameDataKey.CameraPixelAspectRatio);
            float axisSkew = zView.GetFrameDataFloat(receivedFrame, ZView.FrameDataKey.CameraAxisSkew);

            // Update the near and far clip values to account for viewer scale.
            float nearClipPlane = originalNearClipPlane * viewerScale;
            float farClipPlane = originalFarClipPlane * viewerScale;

            // Calculate the camera's transform by transforming its corresponding
            // display space pose matrix to world space.
            Matrix4x4 displayToWorld = coreProxy.GetCoordinateSpaceTransform(ZCoreProxy.CoordinateSpace.Display, ZCoreProxy.CoordinateSpace.World);
            Matrix4x4 worldPoseMatrix = displayToWorld * cameraPoseMatrixInDisplaySpace;

            // Calculate the camera's projection matrix based on the camera intrinsic 
            // and near/far clip values.
            Matrix4x4 projectionMatrix =
                this.ComputeProjectionMatrix(
                    focalLength,
                    principalPointOffsetX,
                    principalPointOffsetY,
                    pixelAspectRatio,
                    axisSkew,
                    (float)_imageWidth,
                    (float)_imageHeight,
                    nearClipPlane,
                    farClipPlane);

            // Update the primary camera's properties (i.e. transform, projection, etc.). 
            _compositorCamera.transform.position = worldPoseMatrix.GetColumn(3);
            _compositorCamera.transform.rotation = Quaternion.LookRotation(worldPoseMatrix.GetColumn(2), worldPoseMatrix.GetColumn(1));
            _compositorCamera.projectionMatrix = projectionMatrix;
            _compositorCamera.cullingMask = _compositorCamera.cullingMask & ~(zView.ARModeIgnoreLayers);
            _compositorCamera.nearClipPlane = nearClipPlane;
            _compositorCamera.farClipPlane = farClipPlane;

            // Copy the compositor camera's properties to the secondary camera.
            _secondaryCamera.CopyFrom(_compositorCamera);


            ///////////////////////////////
            // Box Mask Update
            ///////////////////////////////

            // Enable the box mask to be rendered by the depth camera.
            // Note: The box mask will be disabled immediately after it is rendered
            //       by the AR depth camera so that it isn't inadvertently rendered by
            //       other cameras in the scene.
            _boxMaskObject.SetActive(true);

            // Update the box mask's transform and layer.
            _boxMaskObject.transform.position = coreProxy.GetViewportWorldCenter();
            _boxMaskObject.transform.rotation = coreProxy.GetViewportWorldRotation();
            _boxMaskObject.transform.localScale = Vector3.one * viewerScale;
            _boxMaskObject.layer = zView.ARModeMaskLayer;

            // Update the box mask's size.
            _boxMask.SetSize(zView.ARModeMaskSize);

            // Set the box mask's cutout size to be the size of the viewport
            // in viewport space (meters) since its associated transform's
            // local scale accounts for viewer scale.
            _boxMask.SetCutoutSize(coreProxy.GetViewportWorldSize() / viewerScale);

            // Update the box mask's render queue priority.
            _boxMask.SetRenderQueue(zView.ARModeMaskRenderQueue);


            ///////////////////////////////
            // Scene Render
            ///////////////////////////////

            if (zView.ARModeEnableTransparency)
            {
                this.RenderRGBA(zView);
            }
            else
            {
                this.RenderRGB(zView);   
            }


            // Disable the box mask so that it isn't inadvertently rendered by
            // any other cameras in the scene.
            _boxMaskObject.SetActive(false);

            // Restore the camera's culling mask and near/far clip planes.
            _compositorCamera.cullingMask = originalCullingMask;
            _compositorCamera.nearClipPlane = originalNearClipPlane;
            _compositorCamera.farClipPlane = originalFarClipPlane;
        }

        public override IntPtr GetNativeTexturePtr()
        {
            return _nativeTexturePtr;
        }


        //////////////////////////////////////////////////////////////////
        // Private Methods
        //////////////////////////////////////////////////////////////////

        private void CreateCameras()
        {
            // Create a new Unity camera and disable it to allow for manual 
            // rendering via Camera.Render().
            // NOTE: The camera's rendering path must be set to the forward
            //       rendering path since the current technique for rendering
            //       the augmented reality overlay does not work for deferred
            //       rendering.
            _compositorCamera = this.gameObject.AddComponent<Camera>();
            _compositorCamera.enabled = false;
            _compositorCamera.nearClipPlane = 0.03f;
            _compositorCamera.renderingPath = RenderingPath.Forward;

            // Create the secondary camera.
            GameObject secondaryCameraObject = new GameObject("SecondaryCamera");
            secondaryCameraObject.transform.parent = this.transform;
            secondaryCameraObject.hideFlags = HideFlags.HideAndDontSave;

            _secondaryCamera = secondaryCameraObject.AddComponent<Camera>();
            _secondaryCamera.enabled = false;
            _secondaryCamera.renderingPath = RenderingPath.Forward;
        }

        private void CreateBoxMask()
        {
            // Create the box mask.
            _boxMaskObject = new GameObject("BoxMask");
            _boxMaskObject.transform.parent = this.transform;
            _boxMaskObject.hideFlags = HideFlags.HideAndDontSave;

            _boxMask = _boxMaskObject.AddComponent<BoxMask>();
            _boxMask.SetSize(Vector3.one);

            _boxMaskObject.SetActive(false);
        }

        private void LoadResources()
        {
            // Find and cache the depth render shader.
            _depthRenderShader = Shader.Find("zSpace/zView/DepthRender");
            if (_depthRenderShader == null)
            {
                Debug.LogError("Failed to find the zSpace/zView/DepthRender shader.");
            }

            // Create the RGB compositor material from its associated shader.
            _compositorShaderRGB = Shader.Find("zSpace/zView/CompositorRGB");
            if (_compositorShaderRGB != null)
            {
                _compositorMaterialRGB = new Material(_compositorShaderRGB);
                _compositorMaterialRGB.name = "CompositorRGB";
            }
            else
            {
                Debug.LogError("Failed to find the zSpace/zView/CompositorRGB shader.");
            }

            // Create the RGBA compositor material from its associated shader.
            _compositorShaderRGBA = Shader.Find("zSpace/zView/CompositorRGBA");
            if (_compositorShaderRGBA != null)
            {
                _compositorMaterialRGBA = new Material(_compositorShaderRGBA);
                _compositorMaterialRGBA.name = "CompositorRGBA";
            }
            else
            {
                Debug.LogError("Failed to find the zSpace/zView/CompositorRGBA shader.");
            }
        }

        private void RenderRGB(ZView zView)
        {
            // Update globals for the depth render shader.
            Shader.SetGlobalFloat("_Log2FarPlusOne", (float)Math.Log(_secondaryCamera.farClipPlane + 1, 2));
            
            // Perform a depth render of the mask.
            _secondaryCamera.clearFlags = CameraClearFlags.Color;
            _secondaryCamera.backgroundColor = Color.white;
            _secondaryCamera.cullingMask = (1 << zView.ARModeMaskLayer);
            _secondaryCamera.targetTexture = _maskDepthRenderTexture;
            _secondaryCamera.RenderWithShader(_depthRenderShader, string.Empty);

            // Perform a depth render of the scene excluding the mask
            // layer and any environment layers.
            _secondaryCamera.cullingMask = _compositorCamera.cullingMask & ~(1 << zView.ARModeMaskLayer) & ~(zView.ARModeEnvironmentLayers);
            _secondaryCamera.targetTexture = _nonEnvironmentRenderTexture;
            _secondaryCamera.RenderWithShader(_depthRenderShader, string.Empty);

            // Perform the composite render of the entire scene excluding
            // the mask.
            _compositorCamera.cullingMask = _compositorCamera.cullingMask & ~(1 << zView.ARModeMaskLayer);
            _compositorCamera.targetTexture = _finalRenderTexture;
            _compositorCamera.Render();
        }

        private void RenderRGBA(ZView zView)
        {
            if (zView.ARModeEnvironmentLayers != 0)
            {
                // Update globals for the depth render shader.
                Shader.SetGlobalFloat("_Log2FarPlusOne", (float)Math.Log(_secondaryCamera.farClipPlane + 1, 2));

                // Perform a depth render of the mask.
                _secondaryCamera.clearFlags = CameraClearFlags.Color;
                _secondaryCamera.backgroundColor = Color.white;
                _secondaryCamera.cullingMask = (1 << zView.ARModeMaskLayer);
                _secondaryCamera.targetTexture = _maskDepthRenderTexture;
                _secondaryCamera.RenderWithShader(_depthRenderShader, string.Empty);

                // Render all non-environment objects including the box mask.
                _secondaryCamera.clearFlags = CameraClearFlags.Skybox;
                _secondaryCamera.backgroundColor = MASK_COLOR;
                _secondaryCamera.cullingMask = _compositorCamera.cullingMask & ~(zView.ARModeEnvironmentLayers);
                _secondaryCamera.targetTexture = _nonEnvironmentRenderTexture;
                _secondaryCamera.Render();

                // Perform the composite render of the entire scene excluding
                // the mask.
                _compositorCamera.cullingMask = _compositorCamera.cullingMask & ~(1 << zView.ARModeMaskLayer);
                _compositorCamera.targetTexture = _finalRenderTexture;
                _compositorCamera.Render();
            }
            else
            {
                // Perform a render of the entire scene including the box mask.
                // NOTE: If no environment layers are set, we can optimize this
                //       to a single pass.
                _secondaryCamera.backgroundColor = MASK_COLOR;
                _secondaryCamera.targetTexture = _finalRenderTexture;
                _secondaryCamera.Render();
            }
        }

        private Matrix4x4 ComputeProjectionMatrix(
            float focalLength,
            float principalPointOffsetX,
            float principalPointOffsetY,
            float pixelAspectRatio,
            float axisSkew,
            float imageWidth,
            float imageHeight,
            float nearClip,
            float farClip)
        {
            // Calculate the perspective projection matrix:
            Matrix4x4 perspectiveProjectionMatrix = new Matrix4x4();
            perspectiveProjectionMatrix[0,0] = focalLength;
            perspectiveProjectionMatrix[1,0] = 0.0f;
            perspectiveProjectionMatrix[2,0] = 0.0f;
            perspectiveProjectionMatrix[3,0] = 0.0f;

            // Negate this column to take into account image Y axis pointing down,
            // opposite of OpenGL camera Y axis.
            perspectiveProjectionMatrix[0,1] = -axisSkew;
            perspectiveProjectionMatrix[1,1] = -(focalLength * pixelAspectRatio);
            perspectiveProjectionMatrix[2,1] = 0.0f;
            perspectiveProjectionMatrix[3,1] = 0.0f;

            // Negate this column to take into account OpenGL camera looking down
            // negative Z axis, opposite of convention used in typical camera
            // intrinsics matrix (where camera looks down positive Z axis).
            perspectiveProjectionMatrix[0,2] = -principalPointOffsetX;
            perspectiveProjectionMatrix[1,2] = -principalPointOffsetY;
            perspectiveProjectionMatrix[2,2] = nearClip + farClip;
            perspectiveProjectionMatrix[3,2] = -1.0f;

            perspectiveProjectionMatrix[0,3] = 0.0f;
            perspectiveProjectionMatrix[1,3] = 0.0f;
            perspectiveProjectionMatrix[2,3] = nearClip * farClip;
            perspectiveProjectionMatrix[3,3] = 0.0f;

            Matrix4x4 ndcConversion = Matrix4x4.Ortho(0.0f, imageWidth, imageHeight, 0.0f, nearClip, farClip);

            return ndcConversion * perspectiveProjectionMatrix;
        }


        //////////////////////////////////////////////////////////////////
        // Private Members
        //////////////////////////////////////////////////////////////////

        private static readonly Color MASK_COLOR = new Color(0, 0, 0, 0);

        private Camera        _compositorCamera = null;
        private Camera        _secondaryCamera  = null;

        private Shader        _depthRenderShader    = null;
        private Shader        _compositorShaderRGB  = null;
        private Shader        _compositorShaderRGBA = null;

        private Material      _compositorMaterialRGB  = null;
        private Material      _compositorMaterialRGBA = null;

        private GameObject    _boxMaskObject = null;
        private BoxMask       _boxMask       = null;

        private UInt16        _imageWidth  = 0;
        private UInt16        _imageHeight = 0;

        private RenderTexture _maskDepthRenderTexture      = null;
        private RenderTexture _nonEnvironmentRenderTexture = null;
        private RenderTexture _finalRenderTexture          = null;

        private bool          _isTransparencyEnabled = false;

        private IntPtr        _nativeTexturePtr = IntPtr.Zero;
    }
}