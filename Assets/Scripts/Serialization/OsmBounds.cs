using System.Xml;
using UnityEngine;

class OsmBounds : BaseOsm
{

    public double MinLat { get; private set; }

    public double MaxLat { get; private set; }

    public double MinLon { get; private set; }

    public double MaxLon { get; private set; }

    public Vector3 Centre { get; private set; }

    // minlat="55.8258100" minlon="-4.2610600" maxlat="55.8270900" maxlon="-4.2568500"

    public OsmBounds(XmlNode node)
    {
        // Get the values from the node
        MinLat = XmlConvert.ToDouble(GetAttribute<string>("minlat", node.Attributes));
        MaxLat = XmlConvert.ToDouble(GetAttribute<string>("maxlat", node.Attributes));
        MinLon = XmlConvert.ToDouble(GetAttribute<string>("minlon", node.Attributes));
        MaxLon = XmlConvert.ToDouble(GetAttribute<string>("maxlon", node.Attributes));

        // Create the centre location
        float x = (float)((MercatorProjection.lonToX(MaxLon) + MercatorProjection.lonToX(MinLon)) / 2);
        float y = (float)((MercatorProjection.latToY(MaxLat) + MercatorProjection.latToY(MinLat)) / 2);
        Centre = new Vector3(x, 0, y);
    }
}

