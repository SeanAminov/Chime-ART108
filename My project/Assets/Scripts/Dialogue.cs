using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Chime/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<Line> lines = new List<Line>();

    [System.Serializable]
    public class Line
    {
        public string speaker;
        public Sprite portrait;
        [TextArea(2, 5)]
        public string text;
    }
}
