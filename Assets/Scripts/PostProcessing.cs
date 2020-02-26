using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{
    public Material postProcessingMat;
    public Camera renderCam;
    public RenderTexture rtexture;

    void Start()
    {}

    private void LateUpdate()
    {
        renderCam.targetTexture = rtexture;
        renderCam.Render();
    }

    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, rtexture, postProcessingMat);

    //    Graphics.Blit(source, destination);
    //}
}
