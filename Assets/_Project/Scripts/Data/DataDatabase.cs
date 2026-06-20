using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public enum ContentType
{
    Trunk,
    Chunk,
    Branch,
    Shape,
    Island,
    Clutter,
    Prop,
    Mixed
}

[CreateAssetMenu(fileName = "DataDatabase", menuName = "Scriptable Objects/DataDatabase")]
[ShowOdinSerializedPropertiesInInspector]
public class DataDatabase : SerializedScriptableObject
{
    [Header("Content")]
    [SerializeField] private ContentType _contentType = ContentType.Mixed;
    [OdinSerialize] private IData[] _data;

    public ContentType DatabaseType => _contentType;

    public IEnumerable<T> GetData<T>() where T : ScriptableObject, IData
    {
        if (_data == null) { return Enumerable.Empty<T>(); }

        return _data.OfType<T>();
    }
}
