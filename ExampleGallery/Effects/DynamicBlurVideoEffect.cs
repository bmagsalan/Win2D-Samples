// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace ExampleGallery.Effects
{
    /// <summary>
    /// Win2D Gaussian Blur - http://microsoft.github.io/Win2D/html/T_Microsoft_Graphics_Canvas_Effects_GaussianBlurEffect.htm
    /// BlurAmount (in DIP) - 0 is no blur, default is 3, max for this demo is 12
    /// </summary>
    public sealed class DynamicBlurVideoEffect : IBasicVideoEffect
    {
        private CanvasDevice canvasDevice;
        private IPropertySet configuration;
        public bool IsReadOnly { get { return false; } }

        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties { get { return new List<VideoEncodingProperties>(); } }

        public MediaMemoryTypes SupportedMemoryTypes { get { return MediaMemoryTypes.Gpu; } }

        public bool TimeIndependent { get { return true; } }

        public void Close(MediaEffectClosedReason reason)
        {
            if (canvasDevice != null) canvasDevice.Dispose();
        }

        public void DiscardQueuedFrames()
        {
        }

        public void SetProperties(IPropertySet configuration)
        {
            this.configuration = configuration;
        }

        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device)
        {
            canvasDevice = CanvasDevice.CreateFromDirect3D11Device(device);
        }

        public Matrix5x4 ColorMatrix
        {
            get
            {

                object val;

                if (configuration != null && configuration.TryGetValue("ColorMatrix", out val))
                {
                   return (Matrix5x4)val;

                }
                return new Matrix5x4
                {
                    // Original
                    M11 = 1, M12 = 0, M13 = 0, M14 = 0,
                    M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                    M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                    M41 = 0, M42 = 0, M43 = 0, M44 = 1,
                    M51 = 0, M52 = 0, M53 = 0, M54 = 0
                };
            }
        }

        public void ProcessFrame(ProcessVideoFrameContext context)
        {
            using (CanvasBitmap inputBitmap = CanvasBitmap.CreateFromDirect3D11Surface(canvasDevice, context.InputFrame.Direct3DSurface))
            using (CanvasRenderTarget renderTarget = CanvasRenderTarget.CreateFromDirect3D11Surface(canvasDevice, context.OutputFrame.Direct3DSurface))
            using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
            {

                ColorMatrixEffect colorMatrixEffect = new ColorMatrixEffect
                {
                    Source = inputBitmap
                };

                colorMatrixEffect.ColorMatrix = ColorMatrix;

                ds.DrawImage(colorMatrixEffect);

            }
        }
    }

}
