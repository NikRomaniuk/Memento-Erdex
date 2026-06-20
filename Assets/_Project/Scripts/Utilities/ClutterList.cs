using System.Collections.Generic;
using UnityEngine;

public class ClutterList : MonoBehaviour
{
    [Header("Template Data")]
    [SerializeField] private DataDatabase _templateDb;

    public DataDatabase TemplateDatabase => _templateDb;

    public ClutterSlot[] GetClutterSlots()
    {
        var slots = new List<ClutterSlot>(transform.childCount);
        HashSet<ClutterData> templateLookup = null;

        if (_templateDb == null)
        {
            Debug.LogWarning("Template DataDatabase is not assigned");
        }
        else
        {
            templateLookup = new HashSet<ClutterData>(_templateDb.GetData<ClutterData>());
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            var clutter = child.GetComponent<ClutterManager>();
            if (clutter == null)
            {
                Debug.LogWarning($"ClutterList child '{child.name}' has no ClutterManager. Skipping");
                continue;
            }

            var data = clutter.Data;
            if (data == null)
            {
                Debug.LogWarning($"ClutterManager on '{child.name}' has no ClutterData assigned. Skipping");
                continue;
            }

            bool isTemplateMatch = templateLookup != null && templateLookup.Contains(data);
            Vector3 localPos = child.localPosition;
            Vector2 slotPos = new Vector2(
                Mathf.Round(localPos.x * 100f) / 100f,
                Mathf.Round(localPos.y * 100f) / 100f);

            if (isTemplateMatch)
            {
                slots.Add(new ClutterSlot(false, slotPos, null, data.size));
            }
            else
            {
                slots.Add(new ClutterSlot(true, slotPos, data, data.size));
            }
        }

        return slots.ToArray();
    }
}