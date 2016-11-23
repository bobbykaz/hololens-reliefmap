using UnityEngine;

namespace HackathonMap
{
    public class MapsClient
    {
        public static string BaseUrl = "https://dev.virtualearth.net";

        public static string GetStaticImageUri(string bounds, int pxWidth, int pxHeight)
        {
            string query = string.Format(BaseUrl + "/REST/v1/Imagery/Map/aerial?mapArea={0}&mapSize={1},{2}&key={3}", bounds, pxWidth, pxHeight, Secrets.BingMapsApiKey);
            Debug.Log(query);
            return query;
        }

        public static string GetElevationsUri(string bounds, int rows, int cols)
        {
           string query = string.Format(BaseUrl + "/REST/v1/Elevation/Bounds?bounds={0}&rows={1}&cols={2}&key={3}",
                                        bounds, rows, cols, Secrets.BingMapsApiKey);

            Debug.Log(query);
            return query;
        }
    }
}
