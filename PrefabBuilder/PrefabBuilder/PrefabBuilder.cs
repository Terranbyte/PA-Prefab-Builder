using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PA_PrefabBuilder
{
    public enum EventType
    {
        pos,
        sca,
        rot,
        col
    }

    public enum Easing
    {
        Linear,
        Instant,
        InSine,
        OutSine,
        InOutSine,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        InQuad,
        OutQuad,
        InOutQuad,
        InCirc,
        OutCirc,
        InOutCirc,
        InExpo,
        OutExpo,
        InOutExpo
    }

    public enum PrefabType : byte
    {
        Bombs = 0,
        Bullets = 1,
        Beams = 2,
        Spinners = 3,
        Pulses = 4,
        Characters = 5,
        Misc1 = 6,
        Misc2 = 7,
        Misc3 = 8,
        Misc4 = 9
    }

    public enum ObjectType : byte
    {
        Normal = 0,
        Helper = 1,
        Decoration = 2,
        Empty = 3
    }

    public enum Autokill : byte
    {
        LastKF = 1,
        LastKFOffset = 2,
        FixedTime = 3,
        SongTime = 4,
    }

    public enum Shapes : byte
    {
        Square = 0,
        Circle = 1,
        Triangle = 2,
        Arrow = 3,
        Text = 4,
        Hexagon = 5
    }

    public enum RandomMode : byte
    {
        None,
        Range = 1,
        Select = 3,
        Scale = 4
    }

    /// <summary>
    /// GameObject event.
    /// </summary>
    public class Event
    {

        public EventType Type;
        public Easing Ease = Easing.Linear;
        public RandomMode Random = RandomMode.None;

        public float Time;
        public float X;
        public float Y;
        public float RandomX;
        public float RandomY;
        public float RandomInterval;

        public Event(EventType type, float time)
        {
            Type = type;
            Time = time;
            if (type == EventType.sca)
            {
                X = 2;
                Y = 2;
            }
        }

        public Event(Event Base)
        {
            Type = Base.Type;
            Ease = Base.Ease;
            Random = Base.Random;

            Time = Base.Time;
            X = Base.X;
            Y = Base.Y;
            RandomX = Base.RandomX;
            RandomY = Base.RandomY;
            RandomInterval = Base.RandomInterval;
        }

        public Event Clone()
        {
            Event newEvent = new Event(this);
            return newEvent;
        }

        /// <summary>
        /// Converts this event to a json format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string Out;
            if (Type == EventType.pos || Type == EventType.sca)
            {
                Out = "\n{\n\"t\":\"" +
                      Time +
                      "\",\n\"x\":\"" +
                      X +
                      "\",\n\"y\":\"" +
                      Y +
                      "\",\n\"ct\":\"" +
                      Ease;
                if (Random != RandomMode.None)
                {
                    Out += "\",\n\"r\":\"" +
                           (byte)Random +
                           "\",\n\"rx\":\"" +
                           RandomX +
                           "\",\n\"ry\":\"" +
                           RandomY;
                    if (Random != RandomMode.Select)
                        Out += "\",\"rz\":\"" +
                               RandomInterval;
                }
            }
            else if (Type == EventType.rot)
            {
                Out = "\n{\n\"t\":\"" +
                      Time +
                      "\",\n\"x\":\"" +
                      X +
                      "\",\n\"ct\":\"" +
                      Ease;
                if (Random != RandomMode.None)
                {
                    Out += "\",\n\"r\":\"" +
                           (byte) Random +
                           "\",\n\"rx\":\"" +
                           RandomX +
                           "\",\n\"ry\":\"" +
                           RandomY;
                    if (Random != RandomMode.Select)
                        Out += "\",\n\"rz\":\"" +
                               RandomInterval;
                }
            }
            else
            {
                Out = "\n{\n\"t\":\"" +
                      Time +
                      "\",\n\"x\":\"" +
                      X +
                      "\",\n\"ct\":\"" +
                      Ease;
            }
            Out += "\"\n}";

            return Out;
        }
    }

    /// <summary>
    /// Game object.
    /// </summary>
    public class GameObject
    {
        public string ID = "";
        public string Name = "";
        public string Parent = "";
        public string Text = "Sample text (Original meme ikr)";

        public ObjectType Type = ObjectType.Normal;
        public bool CompressInEditor = false;

        public int Depth = 15;
        public int ShapeVariant;
        public int Bin;
        public int Layer;
        public int ParentSettings = 101;

        public float AutokillTime;
        public float StartTime;
        public float OffsetX;
        public float OffsetY;

        public Autokill AutokillMode = Autokill.LastKFOffset;
        public Shapes Shape = Shapes.Square;

        private readonly List<Event> PosEvents = new List<Event> { new Event(EventType.pos, 0) };
        private readonly List<Event> ScaEvents = new List<Event> { new Event(EventType.sca, 0) };
        private readonly List<Event> RotEvents = new List<Event> { new Event(EventType.rot, 0) };
        private readonly List<Event> ColEvents = new List<Event> { new Event(EventType.col, 0) };

        public GameObject(string ID, string Name, Shapes Shape)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
        }

        public GameObject(string ID, string Name, Shapes Shape, string Parent)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
            this.Parent = Parent;
        }

        public GameObject(string ID, string Name, Shapes Shape, string Parent, int ParentSettings)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
            this.Parent = Parent;
            this.ParentSettings = ParentSettings;
        }

        public GameObject(GameObject Base)
        {
            ID = Base.ID;
            Name = Base.Name;
            Parent = Base.Parent;
            Text = Base.Text;
            
            AutokillMode = Base.AutokillMode;
            AutokillTime = Base.AutokillTime;
            Type = Base.Type;
            
            Depth = Base.Depth;
            ShapeVariant = Base.ShapeVariant;
            Bin = Base.Bin;
            Layer = Base.Layer;
            ParentSettings = Base.ParentSettings;

            StartTime = Base.StartTime;
            OffsetX = Base.OffsetX;
            OffsetY = Base.OffsetY;

            Shape = Base.Shape;

            PosEvents = Base.PosEvents.ConvertAll(GameEvent => new Event(GameEvent.Clone()));
            ScaEvents = Base.ScaEvents.ConvertAll(GameEvent => new Event(GameEvent.Clone()));
            RotEvents = Base.RotEvents.ConvertAll(GameEvent => new Event(GameEvent.Clone()));
            ColEvents = Base.ColEvents.ConvertAll(GameEvent => new Event(GameEvent.Clone()));
        }

        /// <summary>
        /// Sets the starting position of this object.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Random">Random Mode</param>
        /// <param name="RandomX"></param>
        /// <param name="RandomY"></param>
        /// <param name="RandomInterval">Constant distance between random numbers</param>
        public void SetPosition(float X, float Y, float RandomX, float RandomY, RandomMode Random, int RandomInterval = 101)
        {
            PosEvents[0].X = X;
            PosEvents[0].Y = Y;
            PosEvents[0].RandomX = RandomX;
            PosEvents[0].RandomY = RandomY;
            PosEvents[0].Random = Random;
            PosEvents[0].RandomInterval = RandomInterval;
        }

        /// <summary>
        /// Sets the starting scale of this object
        /// </summary>
        /// <param name="X">X Scale</param>
        /// <param name="Y">Y Scale</param>
        /// <param name="Random">Random Mode</param>
        /// <param name="RandomX">X Scale</param>
        /// <param name="RandomY">Y Scale</param>
        /// <param name="RandomInterval">Constant distance between random numbers</param>
        public void SetScale(float X, float Y, float RandomX, float RandomY, RandomMode Random, int RandomInterval = 101)
        {
            ScaEvents[0].X = X;
            ScaEvents[0].Y = Y;
            ScaEvents[0].RandomX = RandomX;
            ScaEvents[0].RandomY = RandomY;
            ScaEvents[0].Random = Random;
            ScaEvents[0].RandomInterval = RandomInterval;
        }

        /// <summary>
        /// Sets the starting rotation of this object
        /// </summary>
        /// <param name="Angle"></param>
        /// <param name="Random">Random Mode</param>
        /// <param name="RandomAngle"></param>
        /// <param name="RandomInterval">Constant distance between random numbers</param>
        public void SetRotation(float Angle, float RandomAngle, RandomMode Random, int RandomInterval = 101)
        {
            RotEvents[0].X = Angle;
            RotEvents[0].RandomX = RandomAngle;
            RotEvents[0].Random = Random;
            RotEvents[0].RandomInterval = RandomInterval;
        }

        /// <summary>
        /// Sets the starting position of this object
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public void SetPosition(float X, float Y)
        {
            PosEvents[0].X = X;
            PosEvents[0].Y = Y;
        }

        /// <summary>
        /// Sets the starting scale of this object
        /// </summary>
        /// <param name="X">X Scale</param>
        /// <param name="Y">Y Scale</param>
        public void SetScale(float X, float Y)
        {
            ScaEvents[0].X = X;
            ScaEvents[0].Y = Y;
        }

        /// <summary>
        /// Sets the starting rotation of this object
        /// </summary>
        /// <param name="Angle"></param>
        public void SetRotation(float Angle)
        {
            RotEvents[0].X = Angle;
        }

        /// <summary>
        /// Sets the starting color of this object
        /// </summary>
        /// <param name="Color"></param>
        public void SetColor(int Color)
        {
            ColEvents[0].X = Color;
        }

        /// <summary>
        /// Adds an event to this object
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Time">Event placement on the event timeline</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Ease">Easing mode for this event</param>
        public void AddEvent(EventType Type, float Time, float X, float? Y, Easing Ease)
        {
            Event e = new Event(Type, Time)
            {
                X = X,
                Ease = Ease
            };

            if (Y != null)
                e.Y = (float)Y;

            switch (Type)
            {
                case EventType.pos:
                    PosEvents.Add(e);
                    break;
                case EventType.sca:
                    ScaEvents.Add(e);
                    break;
                case EventType.rot:
                    RotEvents.Add(e);
                    break;
                case EventType.col:
                    ColEvents.Add(e);
                    break;
            }
        }

        /// <summary>
        /// Adds an event to this object
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Time">Event placement on the event timeline</param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Ease">Easing mode for this event</param>
        /// <param name="Random">Random Mode</param>
        /// <param name="RandomX"></param>
        /// <param name="RandomY"></param>
        /// <param name="RandomInterval">Constant distance between random numbers</param>
        public void AddEvent(EventType Type, float Time, float X, float? Y, Easing Ease, RandomMode Random, float RandomX, float? RandomY, float RandomInterval)
        {
            Event e = new Event(Type, Time)
            {
                X = X,
                Random = Random,
                RandomX = RandomX,
                RandomInterval = RandomInterval,
                Ease = Ease
            };

            if (Y != null)
                e.Y = (float)Y;
            if (RandomY != null)
                e.RandomY = (float)RandomY;

            switch (Type)
            {
                case EventType.pos:
                    PosEvents.Add(e);
                    break;
                case EventType.sca:
                    ScaEvents.Add(e);
                    break;
                case EventType.rot:
                    RotEvents.Add(e);
                    break;
                case EventType.col:
                    ColEvents.Add(e);
                    break;
            }
        }

        /// <summary>
        /// Remove an event at the specified index
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Index"></param>
        public void RemoveEvent(EventType Type, int Index)
        {
            switch (Type)
            {
                case EventType.pos:
                    PosEvents.RemoveAt(Index);
                    break;
                case EventType.sca:
                    ScaEvents.RemoveAt(Index);
                    break;
                case EventType.rot:
                    RotEvents.RemoveAt(Index);
                    break;
                case EventType.col:
                    ColEvents.RemoveAt(Index);
                    break;
            }
        }

        public List<Event> GetEventList(EventType Type)
        {
            switch (Type)
            {
                case EventType.pos:
                    return PosEvents;
                case EventType.sca:
                    return ScaEvents;
                case EventType.rot:
                    return RotEvents;
                default:
                    return ColEvents;
            }
        }

        public GameObject Clone()
        {
            GameObject Object = new GameObject(this);
            return Object;
        }

        /// <summary>
        /// Converts this object to a json format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var StringEvents = "\"events\":{\n\"pos\":[";
            for (int i = 0; i < PosEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += PosEvents[i];
            }

            StringEvents += "\n],\n\"sca\":[";
            for (int i = 0; i < ScaEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += ScaEvents[i];
            }

            StringEvents += "\n],\n\"rot\":[";
            for (int i = 0; i < RotEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += RotEvents[i];
            }

            StringEvents += "\n],\n\"col\":[";
            for (int i = 0; i < ColEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += ColEvents[i];
            }

            var Out = "{\n\"id\":\"" +
                      ID +
                      "\",\n\"name\":\"" +
                      Name +
                      "\",\n\"p\":\"" +
                      Parent +
                      "\",\n\"pt\":\"" +
                      ParentSettings +
                      "\",\n\"d\":\"" +
                      Depth +
                      "\",\n\"akt\":\"" +
                      (int) AutokillMode +
                      "\",\n\"ako\":\"" +
                      AutokillTime +
                      "\",\n\"ot\":\"" +
                      (int) Type +
                      "\",\n\"st\":\"" +
                      StartTime +
                      "\",\n\"shape\":\"" +
                      (int) Shape +
                      "\",\n\"so\":\"" +
                      ShapeVariant +
                      "\",\n\"text\":\"" +
                      Text +
                      "\",\n\"o\":{\n\"x\":\"" +
                      OffsetX +
                      "\",\n\"y\":\"" +
                      OffsetY +
                      "\"\n},\n\"ed\":{\n\"shrink\":\"" +
                      CompressInEditor +
                      "\",\n\"bin\":\"" +
                      Bin +
                      "\",\n\"layer\":\"" +
                      Layer +
                      "\"\n},\n" +
                      StringEvents +
                      "\n]\n}\n}";

            return Out;
        }
    }

    public class PrefabBuilder
    {
        private readonly string Name;
        private readonly PrefabType Type;
        private readonly int Offset;
        public List<GameObject> Objects = new List<GameObject>();

        public PrefabBuilder(string name, PrefabType type, int offset)
        {
            Name = name;
            Type = type;
            Offset = offset;
        }

        /// <summary>
        /// Takes all objects from a prefab and returns them.
        /// Note: If the prefab contains a single object it's returned as a GameObject, otherwise it's returned as a GameObject List.
        /// </summary>
        /// <param name="PrefabPath">Path to the prefab</param>
        public dynamic ImportPrefab(string PrefabPath)
        {
            using (StreamReader sr = new StreamReader(PrefabPath))
            {
                // Get rid of the junk
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();

                List<GameObject> objects = new List<GameObject>();
                string data = sr.ReadToEnd();
                data = data.Insert(0, "{\n");

                JObject rss = JObject.Parse(data);
                JArray objectArray = rss["objects"] as JArray;

                if (objectArray == null)
                {
                    throw new NullReferenceException();
                }

                foreach (var Object in objectArray)
                {
                    JObject Events = Object["events"] as JObject;

                    JArray pos = Events["pos"] as JArray;
                    JArray sca = Events["sca"] as JArray;
                    JArray rot = Events["rot"] as JArray;
                    JArray col = Events["col"] as JArray;

                    byte shape = 99;
                    byte.TryParse((string) Object["so"], out shape);
                    if (shape == 99)
                    {
                        shape = 0;
                    }
                    JObject editor = Object["ed"] as JObject;
                    JObject offset = Object["o"] as JObject;

                    string id = (string) Object["id"], name = (string) Object["name"], parent = (string) Object["p"], text = (string) Object["text"];
                    int? pt = (int?) Object["pt"], d = (int?) Object["d"], so = (int?) Object["so"];
                    pt ??= 101;
                    d ??= 15;
                    so ??= 0;

                    Autokill akt = Autokill.LastKFOffset;
                    float ako = 0;
                    ObjectType ot = ObjectType.Normal;
                    try
                    { 
                        akt = (Autokill)(int) Object["akt"];
                        ako = (float) Object["ako"];
                        ot = (ObjectType)(int) Object["ot"];
                    }
                    catch { }
                    
                    bool? empty = (bool?)Object["empty"], h = (bool) Object["h"];
                    empty ??= false;

                    float? st = (float?) Object["st"];
                    st ??= 0;

                    GameObject obj = new GameObject(id, name, (Shapes)shape, parent, (int)pt);

                    obj.GetEventList(EventType.pos).Clear();
                    obj.GetEventList(EventType.sca).Clear();
                    obj.GetEventList(EventType.rot).Clear();
                    obj.GetEventList(EventType.col).Clear();

                    if (editor != null)
                    {
                        obj.Bin = (int) editor["bin"];
                        obj.Layer = (int) editor["layer"];
                    }

                    if (offset != null)
                    {
                        obj.OffsetX = (float) offset["x"];
                        obj.OffsetY = (float) offset["y"];
                    }

                    obj.AutokillMode = (Autokill) akt;
                    obj.AutokillTime = (float) ako;
                    obj.Depth = (int) d;
                    obj.Type = (ObjectType) ot;
                    obj.ShapeVariant = (int) so;
                    obj.StartTime = (float) st;
                    obj.Text = text;

                    foreach (var Event in pos)
                    {
                        Easing ct = Easing.Linear;
                        Easing.TryParse((string) Event["ct"], out ct);
                        
                        float t = (float) Event["t"], x = (float) Event["x"];
                        float? y = (float?) Event["y"];
                        y ??= 0;
                        
                        RandomMode r = RandomMode.None;
                        RandomMode.TryParse((string) Event["r"], out r);

                        float? rx = (int?) Event["rx"];
                        float? ry = (int?) Event["ry"];
                        float? rz = (int?) Event["rz"];

                        if (r == RandomMode.None)
                        {
                            obj.AddEvent(EventType.pos, t, x, y, ct);
                        }
                        else
                        {
                            obj.AddEvent(EventType.pos, t, x, y, ct, r, (float) rx, ry, (float) rz);
                        }
                    }

                    foreach (var Event in sca)
                    {
                        Easing ct = Easing.Linear;
                        Easing.TryParse((string)Event["ct"], out ct);

                        float t = (float)Event["t"], x = (float)Event["x"];
                        float? y = (float?)Event["y"];
                        y ??= 0;

                        RandomMode r = RandomMode.None;
                        RandomMode.TryParse((string)Event["r"], out r);

                        float? rx = (int?)Event["rx"];
                        float? ry = (int?)Event["ry"];
                        float? rz = (int?)Event["rz"];

                        if (r == RandomMode.None)
                        {
                            obj.AddEvent(EventType.sca, t, x, y, ct);
                        }
                        else
                        {
                            obj.AddEvent(EventType.sca, t, x, y, ct, r, (float)rx, ry, (float)rz);
                        }
                    }

                    foreach (var Event in rot)
                    {
                        Easing ct = Easing.Linear;
                        Easing.TryParse((string)Event["ct"], out ct);

                        float t = (float)Event["t"], x = (float)Event["x"];
                        float? y = (float?)Event["y"];
                        y ??= 0;

                        RandomMode r = RandomMode.None;
                        RandomMode.TryParse((string)Event["r"], out r);

                        float? rx = (int?)Event["rx"];
                        float? rz = (int?)Event["rz"];

                        if (r == RandomMode.None)
                        {
                            obj.AddEvent(EventType.rot, t, x, y, ct);
                        }
                        else
                        {
                            obj.AddEvent(EventType.rot, t, x, y, ct, r, (float)rx, null, (float)rz);
                        }
                    }

                    foreach (var Event in col)
                    {
                        Easing ct = Easing.Linear;
                        Easing.TryParse((string)Event["ct"], out ct);

                        float t = (float)Event["t"], x = (float)Event["x"];
                        float? y = (float?)Event["y"];
                        y ??= 0;
                        
                        obj.AddEvent(EventType.col, t, x, y, ct);
                    }

                    objects.Add(obj);
                }

                if (objects.Count > 1)
                {
                    return objects;
                }

                return objects[0];
            }
        }

        /// <summary>
        /// Finalizes and exports this prefab to a prefab file.
        /// </summary>
        /// <param name="PrefabFolder">Location of all prefabs</param>
        public void Export(string PrefabFolder)
        {
            if (Objects.Count > 0)
            {
                string path = PrefabFolder + "\\" + Name.Replace(' ', '_') + ".lsp";

                if (File.Exists(path))
                    File.Delete(path);
                File.Create(path).Dispose();

                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write(ToString()); // I used this. to make the code a bit nicer to read
                }
            }
            else
            {
                Console.Write("Cannot create empty prefab.");
                throw new Exception();
            }
        }

        /// <summary>
        /// Converts this prefab to a json format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string StrObjects = "";

            for (int i = 0; i < Objects.Count; i++)
            {
                if (i != 0)
                    StrObjects += ",\n";
                StrObjects += Objects[i].ToString();
            }
            
            return "{\n\"name\":\"" +
                   Name +
                   "\",\n\"type\":\"" +
                   (byte) Type +
                   "\",\n\"offset\":\"" +
                   Offset +
                   "\",\n\"objects\":[\n" +
                   StrObjects +
                   "\n]\n}";
        }
    }
}
