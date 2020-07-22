using UnityEngine;

public class VirtualCameraStereo : MonoBehaviour
{

    public Material _sideBySideMaterial = null;

    public RenderTexture _renderTextureLeft = null;
    public RenderTexture _renderTextureRight = null;
    public RenderTexture _renderTextureFinal = null;

    void Awake()
    {

        Shader _sideBySideShader = Shader.Find("ExtendDisplay/SideBySideStereo");
        if (_sideBySideShader != null)
        {
            _sideBySideMaterial = new Material(_sideBySideShader);
            _sideBySideMaterial.name = "ExtendDisplay/SideBySideStereo";
        }
        else
        {
            Debug.LogError("Failed to find the ExtendDisplay/SideBySideStereo shader.");
        }
    }

    public void Start()
    {
        // Grab the image dimensions from the connection settings.
        //int imageWidth = Screen.width;
        //int imageHeight = Screen.height;

        // Configure the side-by-side-stereo material to use the left and right render
        // textures as input.
        _sideBySideMaterial.SetTexture("_LeftTex", _renderTextureLeft);
        _sideBySideMaterial.SetTexture("_RightTex", _renderTextureRight);
    }

    public void Update()
    {
        // Combine left and right textures into final.
        RenderSideBySideStereo();
    }

    private void RenderSideBySideStereo()
    {
        // Draw a full frame quad with the side-by-side-stereo material and
        // final render texture active.  This will cause the left render
        // texture to be rendered (horizontally compressed) into the left
        // side of the final render texture and the right render texture to
        // be rendered (also horizontally compressed) into the right side
        // of the final render texture.

        Graphics.SetRenderTarget(_renderTextureFinal);

        _sideBySideMaterial.SetPass(0);

        GL.PushMatrix();

        GL.LoadOrtho();

        GL.Begin(GL.QUADS);

        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.0f);

        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.0f);

        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);

        GL.End();

        GL.PopMatrix();
    }
}
