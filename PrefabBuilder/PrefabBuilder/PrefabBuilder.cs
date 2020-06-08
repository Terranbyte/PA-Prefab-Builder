using System;
using PA_PrefabBuilder;

class Program
{
    static void Main()
    {
        PrefabBuilder pb = new PrefabBuilder("Wicked box bullets", PrefabType.Bullets, 0);
        double Angle1 = Math.PI / 50;
        double Angle2 = Angle1 * 2;
        float time = 0;
        int i = 0;

        GameObject cp = new GameObject("0", "cp", Shapes.Square);
        cp.Type = ObjectType.Empty;

        pb.Objects.Add(cp);

        while (time < 3.65f)
        {
            GameObject bullet = new GameObject((i + 1).ToString(), "bullet", Shapes.Square, "0");
            bullet.StartTime = time;
            bullet.Bin = 1;

            bullet.SetScale(1.25f, 1.25f);
            bullet.AddEvent(EventType.rot, 2.5f, 360, null, Easing.Linear, RandomMode.Select, -360, null, 0);

            if (i % 3 == 1)
            {
                bullet.AddEvent(EventType.pos, 2.5f, -100 * (float)Math.Sin(Angle2), -100 * (float)Math.Cos(Angle1), Easing.Linear);
            }
            else
            {
                bullet.AddEvent(EventType.pos, 2.5f, -100 * (float)Math.Sin(Angle1), -100 * (float)Math.Cos(Angle2), Easing.Linear);
            }

            ++i;
            time += 0.05f;
            if (i % 3 == 0)
            {
                Angle1 = -Angle1;
                Angle2 = -Angle2;
            }

            pb.Objects.Add(bullet);
        }

        pb.Export(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs");
    }
}
