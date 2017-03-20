using UnityEngine;
using System;
using System.Collections.Generic;

public static class CutOutMask{

    public static Texture2D CutOut(Texture2D wct, Texture2D maskTexture)
    {
        // creating a new texture2d, having same width and height of our webcam texture
        Texture2D destTexture = new Texture2D(wct.width, wct.height, TextureFormat.ARGB32, false);
        // storing pixel data of webcam texture using getpixels
        Color[] textureData = wct.GetPixels();

        // assigning textureData to destTexture
        destTexture.SetPixels(textureData);
        // applying the changes to the texture
        destTexture.Apply();
        // scaling destTexture size to match sampleTexture331x331 that 331*331,
        // so that you dont get unwanted result because of unmatched pixel data 
        TextureScale.Bilinear(destTexture, maskTexture.width, maskTexture.height);
        
        // storing pixel data of destTexture texture using getpixels
        textureData = destTexture.GetPixels();
        
        // storing pixel data of maskTexture texture using getpixels
        Color[] maskPixels = maskTexture.GetPixels();
        // storing pixel data of sampleTexture331x331 texture using getpixels
        Color[] curPixels = destTexture.GetPixels();
        // running a nested for loop, which checks every pixel data in
        // maskPixels and if any pixel in maskPixels matches with maskPixels[0]
        // that is the black portion, it clears that pixel in curPixels
        // where curPixels store our real image from the webcam
        // so thats how we get that oval shape in middle

        Debug.Log(destTexture.width + " " + maskTexture.width + " " + destTexture.height + " " + maskTexture.height);

        //GetPixels Values

        int xMin = 0;
        int yMin = 0;
        int xWidth = 0;
        int yHeight = 0;

        bool countHeight = false;
        bool countWidth = false;

        int index = 0;

        List<Color> newPicture = new List<Color>();
        for (int y = 0; y < maskTexture.height; y++)
        {
            countWidth = false;
            countHeight = false;
            for (int x = 0; x < maskTexture.width; x++)
            {          
                if (maskPixels[index] == maskPixels[0])
                {
                    //Debug.Log("index " + index + " is being checked in curPixels");
                    try
                    {
                        //newPicture.Add(curPixels[index]);
                        curPixels[index] = Color.clear;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("index " + index + " " + e.Message);
                        break;
                    }
                }
                else
                {
                    if (xMin == 0)
                    {
                        xMin = x;
                        yMin = y;
                    }

                    if (!countHeight)
                        yHeight += 1;
                    countHeight = true;
                    if (xWidth == 0)
                        countWidth = true;
                    if (countWidth)
                        xWidth += 1;
                }
                index++;
            }
        }
        Debug.Log("new width = " + xWidth + " new Height =" + yHeight);

        //Create return texture
        Texture2D returnTexture = new Texture2D(xWidth, yHeight);
        Color[] pictureCropped = destTexture.GetPixels(xMin, yMin, xWidth, yHeight);
        returnTexture.SetPixels(pictureCropped);
        returnTexture.Apply();
        return returnTexture;
    }
}
