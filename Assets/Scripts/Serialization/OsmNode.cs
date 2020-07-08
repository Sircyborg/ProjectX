using System.Xml;
using UnityEngine;
class OsmNode : BaseOsm
{
    public ulong ID { get; private set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public float X { get; private set; }

    public float Y { get; private set; }

    public bool IsBusstop { get; private set; }

    public string Name { get; private set; }

    public static implicit operator Vector3 (OsmNode node)
    {
        return new Vector3(node.X, 0f, node.Y);
    }


    public OsmNode(XmlNode node)
    {
        ID = GetAttribute<ulong>("id", node.Attributes);
        Latitude = XmlConvert.ToDouble(GetAttribute<string>("lat", node.Attributes));
        Longitude = XmlConvert.ToDouble(GetAttribute<string>("lon", node.Attributes));

        X = (float)MercatorProjection.lonToX(Longitude);
        Y = (float)MercatorProjection.latToY(Latitude);
        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode t in tags)
        {
            string key = GetAttribute<string>("k", t.Attributes);
            switch (key)
            {
                case "bus":
                    IsBusstop = GetAttribute<string>("v", t.Attributes) == "yes";
                    break;
                case "name":
                    Name = GetAttribute<string>("v", t.Attributes);
                    break;
            }
        }
    }


}

