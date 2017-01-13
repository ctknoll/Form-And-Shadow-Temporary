using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a class that creates colliders that APPROXIMATES the bounds of shadows on an object and gives them 2D colliders.
//this is only absolutely accurate when samp_size = 1, and the class is not optimized for that!
//to be captured accurately, shadows should be THREE TIMES samp_size pixels apart from each other to not be counted as one shadow (safest),
//and shadows should be at least samp_size pixels thick, or risk being skipped over. 
public class ShadowFinder
{
    List<List<Vector2>> shadows; //a list of shadows, which are made of lists of Vector2's representing coordinates of edges
    List<Vector2> visited; //a list of visited sampled pixels
    GameObject obj;
    float val;
    int sample;
    enum Direction {UP,LEFT,RIGHT,DOWN};

    //args = the game object, the discriminatory luma value, and the size differential for sampling
    //the larger the samp_size, the better the performance, but the less the colliders will accurately match the shadows
    //def_args = OBJECT, 100, 8 (TBD?)
    public ShadowFinder(GameObject objec, float luma_value, int samp_size)
    {
        shadows = new List<List<Vector2>>();
        visited = new List<Vector2>();
        obj = objec;
        val = luma_value;
        sample = samp_size;
    }

    //sample pixels to find areas in shadow
    public List<List<Vector2>> getShadowSamples()
    {

        //get the lightmap texture for the game object
        int index = obj.GetComponent<Renderer>().lightmapIndex;
		Debug.Log(index);
        LightmapData lightmapData = LightmapSettings.lightmaps[index];
		Texture2D lightmapTex = lightmapData.lightmapNear;

        //sample every "sample"th pixel of the texture map, and discriminate based on luma value
        for (int i = 0; i < lightmapTex.width; i += sample)
        {
            for (int j = 0; j < lightmapTex.height; j += sample)
            {
                if (visited.Contains(new Vector2(i, j)))
                    continue;
                visited.Add(new Vector2(i, j));
                if(isShadow(lightmapTex, i, j))
                {
                    shadows.Add(new List<Vector2>());
                    generateShadowEdges(lightmapTex, i, j);
                    fill(lightmapTex, i, j);
                }
            }
        }

        return shadows;
    }

    public void generateShadowEdges(Texture2D shadowTexture, int x, int y)
    {
        Vector2 current = new Vector2(x, y);
        //we land in a shadow. We go up until we find an edge (a border or end of the texture). These should always be in shadow
        while (!bordersWhite(shadowTexture, x, y) && !((x + sample) >= shadowTexture.width) && !((x - sample) < 0) && !((y + sample) >= shadowTexture.height) && !((y - sample) < 0))
        {
            current.y += sample;
            visited.Add(current);
        }
        //Add that edge node
        shadows[shadows.Count - 1].Add(current);
        Direction currentDir = Direction.UP;
        //Now we skirt the edge of the object, ending when we get back to the start
        while (current != shadows[shadows.Count - 1][0])
        {
            Vector2 pixelUp = new Vector2(current.x, (current.y + sample < shadowTexture.height ? current.y + sample : shadowTexture.height - 1));
            Vector2 pixelDown = new Vector2(current.x, (current.y - sample >= 0 ? current.y - sample : 0));
            Vector2 pixelLeft = new Vector2((current.x + sample < shadowTexture.width ? current.x + sample : shadowTexture.width - 1), current.y);
            Vector2 pixelRight = new Vector2((current.x - sample >= 0 ? current.x - sample : 0), current.y);

            //check each direction for a node that is in shadow, bordering white(an edge) that we havent visited
            //if there aren't any, you're at a convex corner, and need to go one node outwards
            if (isShadow(shadowTexture, (int)pixelUp.x, (int)pixelUp.y) && bordersWhite(shadowTexture, (int)pixelUp.x, (int)pixelUp.y) && !visited.Contains(pixelUp))
            {
                if(currentDir != Direction.UP)
                {
                    shadows[shadows.Count - 1].Add(current);
                    currentDir = Direction.UP;
                }
                visited.Add(current);
                current.y += sample;
            }
            else if (isShadow(shadowTexture, (int)pixelLeft.x, (int)pixelLeft.y) && bordersWhite(shadowTexture, (int)pixelLeft.x, (int)pixelLeft.y) && !visited.Contains(pixelLeft))
            {
                if (currentDir != Direction.LEFT)
                {
                    shadows[shadows.Count - 1].Add(current);
                    currentDir = Direction.LEFT;
                }
                visited.Add(current);
                current.x -= sample;
            }
            else if (isShadow(shadowTexture, (int)pixelRight.x, (int)pixelRight.y) && bordersWhite(shadowTexture, (int)pixelRight.x, (int)pixelRight.y) && !visited.Contains(pixelRight))
            {
                if (currentDir != Direction.RIGHT)
                {
                    shadows[shadows.Count - 1].Add(current);
                    currentDir = Direction.RIGHT;
                }
                visited.Add(current);
                current.x += sample;
            }
            else if (isShadow(shadowTexture, (int)pixelDown.x, (int)pixelDown.y) && bordersWhite(shadowTexture, (int)pixelDown.x, (int)pixelDown.y) && !visited.Contains(pixelDown))
            {
                if (currentDir != Direction.DOWN)
                {
                    shadows[shadows.Count - 1].Add(current);
                    currentDir = Direction.DOWN;
                }
                visited.Add(current);
                current.y -= sample;
            }
            else
            {
                switch(currentDir)
                {
                    case Direction.UP: current.y += sample; break;
                    case Direction.DOWN: current.y -= sample; break;
                    case Direction.LEFT: current.x -= sample; break;
                    case Direction.RIGHT: current.x += sample; break;
                    default: current.y += sample; break;
                }
            }
        }
        shadows[shadows.Count - 1].Add(current);
    }

    //just checks if the given pixel is in shadow
    public bool isShadow(Texture2D shadowTexture, int x, int y)
    {
        Color surfaceColor = shadowTexture.GetPixelBilinear(x, y);
        float brightness = (surfaceColor.r + surfaceColor.r + surfaceColor.b + surfaceColor.g + surfaceColor.g + surfaceColor.g) / 6;
        if (brightness <= val)
        {
            return true;
        }
        else return false;
    }
    
    
    public bool bordersWhite(Texture2D shadowTexture, int x, int y)
    {
        Vector2 pixelUp = new Vector2(x, (y + sample < shadowTexture.height ? y + sample : shadowTexture.height - 1));
        Vector2 pixelDown = new Vector2(x, (y - sample >= 0 ? y - sample : 0));
        Vector2 pixelLeft = new Vector2((x + sample < shadowTexture.width ? x + sample : shadowTexture.width - 1), y);
        Vector2 pixelRight = new Vector2((x - sample >= 0 ? x - sample : 0), y);
        Color surfaceColorUp, surfaceColorDown, surfaceColorLeft, surfaceColorRight;
       
        if (!visited.Contains(pixelUp))
        {
            visited.Add(pixelUp);
            surfaceColorUp = shadowTexture.GetPixelBilinear(pixelUp.x, pixelUp.y);
            float brightness = (surfaceColorUp.r + surfaceColorUp.r + surfaceColorUp.b + surfaceColorUp.g + surfaceColorUp.g + surfaceColorUp.g) / 6;
            if (brightness > val)
            {
                return true;
            }
        }

        if (!visited.Contains(pixelDown))
        {
            visited.Add(pixelDown);
            surfaceColorDown = shadowTexture.GetPixelBilinear(pixelDown.x, pixelDown.y);
            float brightness = (surfaceColorDown.r + surfaceColorDown.r + surfaceColorDown.b + surfaceColorDown.g + surfaceColorDown.g + surfaceColorDown.g) / 6;
            if (brightness > val)
            {
                return true;
            }
        }

        if (!visited.Contains(pixelLeft))
        {
            visited.Add(pixelLeft);
            surfaceColorLeft = shadowTexture.GetPixelBilinear(pixelLeft.x, pixelLeft.y);
            float brightness = (surfaceColorLeft.r + surfaceColorLeft.r + surfaceColorLeft.b + surfaceColorLeft.g + surfaceColorLeft.g + surfaceColorLeft.g) / 6;
            if (brightness > val)
            {
                return true;
            }
        }

        if (!visited.Contains(pixelRight))
        {
            visited.Add(pixelRight);
            surfaceColorRight = shadowTexture.GetPixelBilinear(pixelRight.x, pixelRight.y);
            float brightness = (surfaceColorRight.r + surfaceColorRight.r + surfaceColorRight.b + surfaceColorRight.g + surfaceColorRight.g + surfaceColorRight.g) / 6;
            if (brightness > val)
            {
                return true;
            }
        }
        return false;
    }

    //fills nodes inside the shadow so getShadowSamples doesnt accidentally multicount shadows
    public void fill(Texture2D shadowTexture, int x, int y)
    {
        Vector2 pixelUp = new Vector2(x, (y + sample < shadowTexture.height ? y + sample : shadowTexture.height - 1));
        Vector2 pixelDown = new Vector2(x, (y - sample >= 0 ? y - sample : 0));
        Vector2 pixelLeft = new Vector2((x + sample < shadowTexture.width ? x + sample : shadowTexture.width - 1), y);
        Vector2 pixelRight = new Vector2((x - sample >= 0 ? x - sample : 0), y);

        if (!visited.Contains(pixelUp) && isShadow(shadowTexture, (int)pixelUp.x, (int)pixelUp.y))
        {
            visited.Add(pixelUp);
            fill(shadowTexture, (int)pixelUp.x, (int)pixelUp.y);
        }
        if (!visited.Contains(pixelDown) && isShadow(shadowTexture, (int)pixelDown.x, (int)pixelDown.y))
        {
            visited.Add(pixelDown);
            fill(shadowTexture, (int)pixelDown.x, (int)pixelDown.y);
        }
        if (!visited.Contains(pixelLeft) && isShadow(shadowTexture, (int)pixelLeft.x, (int)pixelLeft.y))
        {
            visited.Add(pixelLeft);
            fill(shadowTexture, (int)pixelLeft.x, (int)pixelLeft.y);
        }
        if (!visited.Contains(pixelRight) && isShadow(shadowTexture, (int)pixelRight.x, (int)pixelRight.y))
        {
            visited.Add(pixelRight);
            fill(shadowTexture, (int)pixelRight.x, (int)pixelRight.y);
        }
    }

    //creates a GameObject with a collider matching the shadow
    public GameObject generateCollidableShadow(List<Vector2> nodes)
    {
        GameObject obj = new GameObject();
        EdgeCollider2D col = obj.AddComponent<EdgeCollider2D>();
        nodes.Add(nodes[0]); //close the shadow's collider
        col.points = nodes.ToArray();
        return obj;
    }
}