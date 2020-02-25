using System;
using System.Collections.Generic;
using System.IO;

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
        InOutExpo,
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

        public float Time, X = 0, Y = 0, RandomX = 0, RandomY = 0, RandomInterval = 0;

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
                           (byte)Random +
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
        public string ID = "", Name = "", Parent = "", Text = "Sample text (Original meme ikr)";
        public bool Helper = false, Autokill = true, Empty = false;

        public int
            Depth = 15,
            ShapeVariant = 0,
            Bin = 0,
            Layer = 0,
            ParentSettings = 101;
        public float 
            StartTime = 0,
            OffsetX = 0,
            OffsetY = 0;

        public Shapes Shape = Shapes.Square;

        readonly List<Event> PosEvents = new List<Event> { new Event(EventType.pos, 0) };
        readonly List<Event> ScaEvents = new List<Event> { new Event(EventType.sca, 0) };
        readonly List<Event> RotEvents = new List<Event> { new Event(EventType.rot, 0) };
        readonly List<Event> ColEvents = new List<Event> { new Event(EventType.col, 0) };

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

        /// <summary>
        /// Sets the starting position of this object.
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Random">Random Mode</param>
        /// <param name="RandomX"></param>
        /// <param name="RandomY"></param>
        /// <param name="RandomInterval">Constant distance between random numbers</param>
        public void SetPosition(float X, float Y, RandomMode Random, float RandomX, float RandomY, float RandomInterval)
        {
            PosEvents[0].X = X;
            PosEvents[0].Y = Y;
            PosEvents[0].Random = Random;
            PosEvents[0].RandomX = RandomX;
            PosEvents[0].RandomY = RandomY;
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
        public void SetScale(float X, float Y, RandomMode Random, float RandomX, float RandomY, float RandomInterval)
        {
            ScaEvents[0].X = X;
            ScaEvents[0].Y = Y;
            ScaEvents[0].Random = Random;
            ScaEvents[0].RandomX = RandomX;
            ScaEvents[0].RandomY = RandomY;
            ScaEvents[0].RandomInterval = RandomInterval;
        }

        /// <summary>
        /// Sets the starting rotation of this object
        /// </summary>
        /// <param name="Angle"></param>
        /// <param name="Random">Random Mode</param>
        /// <param name="RandomAngle"></param>
        /// <param name="RandomInterval">Constant distance between random numbers</param>
        public void SetRotation(float Angle, RandomMode Random, float RandomAngle, float RandomInterval)
        {
            RotEvents[0].X = Angle;
            RotEvents[0].Random = Random;
            RotEvents[0].RandomX = RandomAngle;
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
                      "\",\n\"h\":\"" +
                      Helper +
                      "\",\n\"ak\":\"" +
                      Autokill +
                      "\",\n\"empty\":\"" +
                      Empty +
                      "\",\n\"st\":\"" +
                      StartTime +
                      "\",\n\"shape\":\"" +
                      (int)Shape +
                      "\",\n\"so\":\"" +
                      ShapeVariant +
                      "\",\n\"text\":\"" +
                      Text +
                      "\",\n\"o\":{\"x\":\"" +
                      OffsetX +
                      "\",\n\"y\":\"" +
                      OffsetY +
                      "\"},\n\"ed\":{\n\"bin\":\"" +
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
        private string ImportedObjects = "";
        private readonly int Offset;
        public List<GameObject> Objects = new List<GameObject>();

        public PrefabBuilder(string name, PrefabType type, int offset)
        {
            Name = name;
            Type = type;
            Offset = offset;
        }
        
        /// <summary>
        /// Takes all objects from an existing prefabs and places them in the current prefab.
        /// Note: All imported objects are json formatted.
        /// </summary>
        /// <param name="PrefabPath">Path to the prefab</param>
        public void ImportPrefab(string PrefabPath)
        {
            using (StreamReader sr = new StreamReader(PrefabPath))
            {
                // Get rid of the junk
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();

                string data = sr.ReadToEnd();
                if (ImportedObjects.Length != 0)
                    ImportedObjects += ",\n";
                ImportedObjects += data.Remove(data.Length - 3, 3);
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
                    sw.Write(this.ToString()); // I used this. to make the code a bit nicer to read
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

            if (Objects.Count > 0)
                StrObjects += ",\n";
            StrObjects += ImportedObjects;

            return "{\n\"name\":\"" +
                   Name +
                   "\",\n\"type\":\"" +
                   (byte)Type +
                   "\",\n\"offset\":\"" +
                   Offset +
                   "\",\n\"objects\":[\n" +
                   StrObjects +
                   "\n]\n}".Replace("\\", string.Empty);
        }
    }
}
