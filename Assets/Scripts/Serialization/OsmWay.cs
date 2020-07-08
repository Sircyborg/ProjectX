using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

class OsmWay : BaseOsm
{
    public ulong ID { get; private set; }

    public List<ulong> NodeIDs { get; private set; }

    public bool Visible { get; private set; }

    public bool IsBoundary { get; private set; }

    public bool IsRoad { get; private set; }

    public float Height { get; private set; }

    public bool IsBuilding { get; private set; }

    public string Name { get; private set; }

    public int Lanes { get; private set; }

    public OsmWay(XmlNode node)
    {
        NodeIDs = new List<ulong>();
        Visible = GetAttribute<bool>("visible", node.Attributes);
        ID = GetAttribute<ulong>("id", node.Attributes);
        Height = 3.0f;
        XmlNodeList nds = node.SelectNodes("nd");
        foreach (XmlNode n in nds)
        {
            ulong refNo = GetAttribute<ulong>("ref", n.Attributes);
            NodeIDs.Add(refNo);
        }

        // TODO: Determine what type of way it is; is it a road / boundary

        if (NodeIDs.Count>1)
        {
            IsBoundary = NodeIDs[0] == NodeIDs[NodeIDs.Count -  1];
        }
        XmlNodeList tags = node.SelectNodes("tag");
        foreach (XmlNode t in tags)
        {
            string key = GetAttribute<string>("k", t.Attributes);
            switch (key)
            {
                case "building:levels":
                    Height = 3.0f * GetAttribute<float>("v", t.Attributes);
                    break;
                case "height":
                // TODO: Highway things
                    Height = 0.3048f * GetAttribute<float>("v", t.Attributes);
                    break;
                case "building":
                    IsBuilding = GetAttribute<string>("v", t.Attributes) == "yes" || GetAttribute<string>("v", t.Attributes) == "garage";
                    break;
                case "highway":
                    IsRoad = true;
                    break;
                case "name":
                    Name = GetAttribute<string>("v", t.Attributes);
                    break;
                case "lanes":
                    Lanes = GetAttribute<int>("v", t.Attributes);
                    break;

            }
        }

    }

}

