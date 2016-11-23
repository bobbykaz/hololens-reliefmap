using UnityEngine;
using System.Collections;
using HackathonMap;

public class TerrainDownloader : MonoBehaviour
{
    private Vector2 GrandCanyon = new Vector2(36.100908f, -112.135685f);
    private Vector2 MtWhitney = new Vector2(36.587499f, -118.324287f);
    private Vector2 NiagraFalls = new Vector2(43.096786f,-79.064320f);
    private Vector2 Everest = new Vector2(27.97f,86.90f);
    private Vector2 MtWilson = new Vector2(34.225456f, -118.064961f);
    private Vector2 MonoPass = new Vector2(37.100867f, -118.522542f);
    private Vector2 Hawaii = new Vector2(19.474691f, -155.588260f);
    private Vector2 GobiDesert = new Vector2(39.971472f, 103.363165f);

    private Vector2[] Coords;
    private int CoordIndex;
    private float boundingBoxOffset = 0.05f;

    private bool imgSet;
    private bool elevSet;
    private WWW ImgResource;
    private WWW ElevationResource;

    private int rowsAndCols = 32;
    private int terrainDim = 32;

    private float[,] elevationData;
    private float minElevation = 0f;

    void Start()
    {
        Coords = new Vector2[] { Hawaii, MtWhitney, GrandCanyon, NiagraFalls, MtWilson, MonoPass, GobiDesert, Everest };
        CoordIndex = 0;
        ResetTerrainHeights();
        SelectNewLocation(Coords[CoordIndex]);
    }

    public void OnSelect()
    {
        if (!TapToPlaceParent.Placing && imgSet && elevSet)
        {
            CoordIndex = (CoordIndex + 1) % Coords.Length;
            SelectNewLocation(Coords[CoordIndex]);
        }
    }

    private void SelectNewLocation(Vector2 position)
    {
        string bounds = string.Format("{0},{1},{2},{3}", position.x - boundingBoxOffset, position.y - boundingBoxOffset, position.x + boundingBoxOffset, position.y + boundingBoxOffset);
        ImgResource = new WWW(MapsClient.GetStaticImageUri(bounds, 512, 512));
        elevationData = new float[rowsAndCols, rowsAndCols];
        ElevationResource = new WWW(MapsClient.GetElevationsUri(bounds, rowsAndCols, rowsAndCols));

        imgSet = false;
        elevSet = false;
    }

    private void ResetTerrainHeights()
    {
        Terrain terr = GetComponent<Terrain>();
        var nRows = terrainDim;
        var nCols = terrainDim;

        var heights = new float[nRows, nCols];
        for (var j = 0; j < nRows; j++)
            for (var i = 0; i < nCols; i++)
                heights[j, i] = 0f;
        terr.terrainData.SetHeights(0, 0, heights);
    }

    private void SetTerrainHeights()
    {
        Terrain terr = GetComponent<Terrain>();
        var nRows = terrainDim;
        var nCols = terrainDim;

        int modifier = terrainDim / rowsAndCols;

        var heights = new float[nRows, nCols];
        for (var j = 0; j < nRows; j++)
            for (var i = 0; i < nCols; i++)
            {
                heights[j, i] = 
                    (elevationData[j / modifier, i / modifier] - minElevation) / 2500f;
            }
        terr.terrainData.SetHeights(0, 0, heights);
    }

    private void SetImageIfNeeded()
    {
        if (elevSet && !imgSet && ImgResource.isDone)
        {
            Terrain terr = GetComponent<Terrain>();
            var tdata = terr.terrainData;

            Debug.Log("Num splats" + tdata.splatPrototypes.Length);
            Debug.Log(tdata.splatPrototypes);

            SplatPrototype[] newSplats = new SplatPrototype[1];

            newSplats[0] = new SplatPrototype();
            newSplats[0].texture = ImgResource.texture;
            newSplats[0].tileSize = new Vector2(2, 2);
            newSplats[0].tileOffset = new Vector2(0, 0);

            tdata.splatPrototypes = newSplats;
            tdata.RefreshPrototypes();
            terr.Flush();
            imgSet = true;
        }
    }

    private void SetElevationsIfNeeded()
    {
        if (!elevSet && ElevationResource != null && ElevationResource.isDone)
        {
            Debug.Log("parsing download");
            var N = SimpleJSON.JSON.Parse(ElevationResource.text);
            var elevations = N["resourceSets"][0]["resources"][0]["elevations"].AsArray;

            ResetTerrainHeights();

            minElevation = elevations[0].AsFloat;

            for (int x = 0; x < elevations.Count; x++)
            {
                elevationData[x / rowsAndCols, x % rowsAndCols] = elevations[x].AsFloat;
                minElevation = Mathf.Min(minElevation, elevationData[x / rowsAndCols, x % rowsAndCols]);
            }

            SetTerrainHeights();
            elevSet = true;
        }
    }

    void Update()
    {
        SetImageIfNeeded();
        SetElevationsIfNeeded();
    }
}
