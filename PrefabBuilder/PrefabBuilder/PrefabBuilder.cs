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
    /// Object event
    /// </summary>
    public class Event
    {
        public EventType Type;
        public Easing Ease = Easing.Linear;
        public RandomMode Random = RandomMode.None;

        public float Time, X = 0, Y = 0;

        public int RandomX = 0, RandomY = 0, RandomInterval = 0;

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
        /// Converts this event to a json format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string Out;
            if (Type == EventType.pos || Type == EventType.sca)
            {
                Out = "{\"t\":\"" +
                      Time +
                      "\",\"x\":\"" +
                      X +
                      "\",\"y\":\"" +
                      Y +
                      "\",\"ct\":\"" +
                      Ease;
                if (Random != RandomMode.None)
                {
                    Out += "\",\"r\":\"" +
                           (byte)Random +
                           "\",\"rx\":\"" +
                           RandomX +
                           "\",\"ry\":\"" +
                           RandomY;
                    if (Random != RandomMode.Select)
                        Out += "\",\"rz\":\"" +
                               RandomInterval;
                }
            }
            else if (Type == EventType.rot)
            {
                Out = "{\"t\":\"" +
                      Time +
                      "\",\"x\":\"" +
                      X +
                      "\",\"ct\":\"" +
                      Ease;
                if (Random != RandomMode.None)
                {
                    Out += "\",\"r\":\"" +
                           (byte)Random +
                           "\",\"rx\":\"" +
                           RandomX +
                           "\",\"ry\":\"" +
                           RandomY;
                    if (Random != RandomMode.Select)
                        Out += "\",\"rz\":\"" +
                               RandomInterval;
                }
            }
            else
            {
                Out = "{\"t\":\"" +
                      Time +
                      "\",\"x\":\"" +
                      X +
                      "\",\"ct\":\"" +
                      Ease;
            }
            Out += "\"}";

            return Out;
        }
    }

    /// <summary>
    /// Game object
    /// </summary>
    public class Object
    {
        public string ID = "", Name = "", Parent = "", Text = "Sample text (Original meme ikr)";
        public bool Helper = false, Autokill = true, Empty = false;
        public int
            Depth = 15,
            StartTime = 0,
            ShapeVariant = 0,
            OffsetX = 0,
            OffsetY = 0,
            Bin = 0,
            Layer = 0,
            ParentSettings = 101;

        public Shapes Shape = Shapes.Square;

        readonly List<Event> PosEvents = new List<Event> { new Event(EventType.pos, 0) };
        readonly List<Event> ScaEvents = new List<Event> { new Event(EventType.sca, 0) };
        readonly List<Event> RotEvents = new List<Event> { new Event(EventType.rot, 0) };
        readonly List<Event> ColEvents = new List<Event> { new Event(EventType.col, 0) };

        public Object(string ID, string Name, Shapes Shape)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
        }

        public Object(string ID, string Name, Shapes Shape, string Parent)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
            this.Parent = Parent;
        }

        public Object(string ID, string Name, Shapes Shape, string Parent, int ParentSettings)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
            this.Parent = Parent;
            this.ParentSettings = ParentSettings;
        }

        public Object(string ID, string Name, Shapes Shape, float StartX, float StartY)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
            PosEvents[0].X = StartX;
            PosEvents[0].Y = StartY;
        }

        public Object(string ID, string Name, Shapes Shape, float StartX, float StartY, string Parent)
        {
            this.ID = ID;
            this.Name = Name;
            this.Parent = Parent;
            this.Shape = Shape;
            PosEvents[0].X = StartX;
            PosEvents[0].Y = StartY;
        }

        public Object(string ID, string Name, Shapes Shape, float StartX, float StartY, string Parent, int ParentSettings)
        {
            this.ID = ID;
            this.Name = Name;
            this.Shape = Shape;
            this.Parent = Parent;
            this.ParentSettings = ParentSettings;
            PosEvents[0].X = StartX;
            PosEvents[0].Y = StartY;
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
        /// <param name="X"></param>
        /// <param name="Y"></param>
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
        /// <param name="Time"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Ease"></param>
        public void AddEvent(EventType Type, float Time, float X, float? Y, Easing Ease)
        {
            Event e = new Event(Type, Time);

            e.X = X;
            if (Y != null)
                e.Y = (float)Y;
            e.Ease = Ease;

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
        /// <param name="Time"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Ease"></param>
        public void AddEvent(EventType Type, float Time, float X, float? Y, Easing Ease, RandomMode Random, int RandomX, int RandomY)
        {
            Event e = new Event(Type, Time);

            e.X = X;
            if (Y != null)
                e.Y = (float)Y;
            e.Ease = Ease;
            e.Random = Random;
            e.RandomX = RandomX;
            e.RandomY = RandomY;

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
            var StringEvents = "\"events\":{\"pos\":[";
            for (int i = 0; i < PosEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += PosEvents[i];
            }

            StringEvents += "],\"sca\":[";
            for (int i = 0; i < ScaEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += ScaEvents[i];
            }

            StringEvents += "],\"rot\":[";
            for (int i = 0; i < RotEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += RotEvents[i];
            }

            StringEvents += "],\"col\":[";
            for (int i = 0; i < ColEvents.Count; i++)
            {
                if (i != 0)
                    StringEvents += ",";
                StringEvents += ColEvents[i];
            }

            var Out = "{\"id\":\"" +
                      ID +
                      "\",\"name\":\"" +
                      Name +
                      "\",\"p\":\"" +
                      Parent +
                      "\",\"pt\":\"" +
                      ParentSettings +
                      "\",\"d\":\"" +
                      Depth +
                      "\",\"h\":\"" +
                      Helper +
                      "\",\"ak\":\"" +
                      Autokill +
                      "\",\"empty\":\"" +
                      Empty +
                      "\",\"st\":\"" +
                      StartTime +
                      "\",\"shape\":\"" +
                      (int)Shape +
                      "\",\"so\":\"" +
                      ShapeVariant +
                      "\",\"text\":\"" +
                      Text +
                      "\",\"o\":{\"x\":\"" +
                      OffsetX +
                      "\",\"y\":\"" +
                      OffsetY +
                      "\"},\"ed\":{\"bin\":\"" +
                      Bin +
                      "\",\"layer\":\"" +
                      Layer +
                      "\"}," +
                      StringEvents +
                      "]}}";

            return Out;
        }
    }

    public class PrefabBuilder
    {
        private string Name;
        private PrefabType Type;
        private int Offset;
        public List<Object> Objects = new List<Object>();

        public PrefabBuilder(string name, PrefabType type, int offset)
        {
            Name = name;
            Type = type;
            Offset = offset;
        }

        /// <summary>
        /// Finalizes and exports this prefab to a prefab file
        /// </summary>
        /// <param name="PrefabFolder"></param>
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
        /// Converts this prefab to a json format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string StrObjects = "";

            for (int i = 0; i < Objects.Count; i++)
            {
                if (i != 0)
                    StrObjects += ',';
                StrObjects += Objects[i].ToString();
            }

            return "{\"name\":\"" +
                   Name +
                   "\",\"type\":\"" +
                   (byte)Type +
                   "\",\"offset\":\"" +
                   Offset +
                   "\",\"objects\":[" +
                   StrObjects +
                   "]}".Replace("\\", string.Empty);
        }
    }
}
