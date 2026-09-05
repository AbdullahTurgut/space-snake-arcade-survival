using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerParts : MonoBehaviour
{
    public class Marker
    {
        public Vector3 pos;
        public Quaternion rot;

        public Marker(Vector3 pos, Quaternion rot)
        {
            this.pos = pos;
            this.rot = rot;
        }


    }

    public List<Marker> markerList = new List<Marker>();

    private void FixedUpdate()
    {
        UpdateMarkerList();
    }

    public void UpdateMarkerList()
    {
        markerList.Add(new Marker(transform.position,transform.rotation));

    }

    public void ClearMarkerList()
    {
        markerList.Clear();
        markerList.Add(new Marker(transform.position, transform.rotation));
    }
}
