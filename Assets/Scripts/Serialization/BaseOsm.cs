using System;
using System.Xml;
using UnityEngine;

class BaseOsm
{

    protected T GetAttribute<T>(string attrName, XmlAttributeCollection attributes)
    {
        // TODO: We are going to assume 'attrName' exists in the collection
        //Debug.Log(attributes[attrName]);
        string strValue = attributes[attrName].Value;
        return (T)Convert.ChangeType(strValue, typeof(T));
    }
}

