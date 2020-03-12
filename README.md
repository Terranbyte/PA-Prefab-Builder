# Project Arrhythmia Prefab Builder
A C# library used to create prefabs in the game [Project Arrhythmia](https://store.steampowered.com/app/440310/Project_Arrhythmia/).

**Importaint!**
This library is finished, but it most likely bugs.
If you encounter any bugs/mistakes or you have a feature request, please DM me on Discord (Terranbyte#1691)
## Installing the library (VS 2019)

 1. Download the latest version of the DLL from [here](https://github.com/Terranbyte/PA-Prefab-Builder/releases/download/v1.0/PrefabBuilder.dll).
 2. Right-click the References tab in your solution explorer.
 3. Click "Add reference...".
 4. Locate and select the DLL.
 5. Click ok.



# Getting Started

Let's create a prefab with a single object that moves.
First a PrefabBuilder object needs to be defined, it takes in a prefab name, type and offset.
```cs
PrefabBuilder pb = new PrefabBuilder("Name", PrefabType.Misc1, 0);
```
after this an object needs to be defined. The Object class has three constructor overloads where the one below is the simplest, needing an object ID, name and shape to be defined.
```cs
GameObject obj = new Object("0", "My First Object", Shapes.Square);
```
To edit the start events of an object use the following functions
```cs
obj.SetPosition();
obj.SetScale();
obj.SetRotation();
obj.SetColor();
```
When adding events to a prefab, call the AddEvent method on a GameObject. The first overload will take an event type, timestamp, X value, Y value (nullable) and an easing type.
```cs
obj.AddEvent(EventType.pos, 3, 15, -15, Easing.Linear);
```
Events can also be removed using the RemoveEvent method.
```cs
obj.RemoveEvent(EventType.pos, 1);
```
When exporting a prefab, use the Export function on your PrefabBuilder object with the prefab folder as the parameter
```cs
pb.Export(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs");
```
The ImportPrefab method has been remade to now return GameObjects that can be used in your code. It takes a path to a prefab file (.lsp) and depending on the prefab, it returns either a single GameObject or a list of GameObjects.
```cs
GameObject SingleObject = pb.ImportPrefab(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs\Prefab1.lsp");
List<GameObject> MultipleObjects = pb.ImportPrefab(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs\Prefab2.lsp");
```
You can also modify existing events but calling to the GetEventList method on a GameObject, it will return a reference the that objects event list with the type of the entered parameter. I don't recommend doing this though because it complicates the prefab creation.
```cs
obj.GetEventList(EventType.pos);
```
# Examples
**Generating a single object**
```cs
using PA_PrefabBuilder;

class Program
{
	static void Main()
	{
		PrefabBuilder pb = new PrefabBuilder("Generated prefab", PrefabType.Misc1, 0);
		GameObject obj = new Object("0", "My First Object", Shapes.Square);
		pb.Objects.Add(obj);
		
		pb.Export(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs");
	}	
}
```

**Creating a moving arrow with random scale**

```cs
using PA_PrefabBuilder;

class Program
{
	static void Main()
	{
		PrefabBuilder pb = new PrefabBuilder("Moving Arrow", PrefabType.Bullets, 0);
		Object obj = new Object("0", "Arrow", Shapes.Arrow);

		obj.ShapeVariant = 1;
		obj.SetRotation(90);
		obj.SetScale(4, 4, RandomMode.Range, 8, 8, 0);
		
		obj.AddEvent(EventType.pos, 2, 10, 0, Easing.InOutSine);

		pb.Objects.Add(obj);

		pb.Export(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs");
	}	
}
```
**Creating bomb bullets**
```cs
using System;
using PA_PrefabBuilder;

class Program
{
	static void Main()
	{
		PrefabBuilder pb = new PrefabBuilder("Bomb Bullets", PrefabType.Bullets, 0);
		GameObject obj;
		float Angle = 1;

		for (int i = 0; i < 20; i++)
		{
			obj = new GameObject(i.ToString(), "Bullet", Shapes.Square);
			obj.ShapeVariant = 1;

			obj.AddEvent(EventType.rot, 4, -360, null, Easing.Linear, RandomMode.Select, 360, null);
			obj.AddEvent(EventType.pos, 4, (float)Math.Sin(Angle) * 50, (float)Math.Cos(Angle) * 50, Easing.OutSine);

			pb.Objects.Add(obj);
			Angle += (float) Math.PI / 10; // Adds up to 2PI
		}

		pb.Export(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs");
	}
}
```
**Creating bomb bullets from a design prefab**

The design prefab looks like this:

![](https://cdn.discordapp.com/attachments/493523075392864261/687584112084910110/Project_Arrhythmia_12_03_2020_09_50_22_2.png)
```cs
using System;
using System.Collections.Generic;
using PA_PrefabBuilder;

class Program
{
    static void Main()
    {
        PrefabBuilder pb = new PrefabBuilder("Detailed Bomb Bullets", PrefabType.Bullets, 0);
        float Angle = 0;

        // Get the bullet design
        List<GameObject> bullet = pb.ImportPrefab(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs\bullet_design.lsp");

        // Make some changes to the design
        bullet[0].Shape = Shapes.Triangle;
        bullet[0].ShapeVariant = 1;
        bullet[1].GetEventList(EventType.rot)[1].Time = 6;
        bullet[1].GetEventList(EventType.rot)[1].X *= 2;

        for (int i = 0; i < 20; i++)
        {
            GameObject obj1 = bullet[0].Clone();
            obj1.ID = i.ToString();

            GameObject obj2 = bullet[1].Clone();
            obj2.ID = (i + 20).ToString();
            obj2.Parent = obj1.ID;

            obj1.AddEvent(EventType.pos, 6, (float)Math.Sin(Angle) * 50, (float)Math.Cos(Angle) * 50, Easing.OutSine);
            obj1.SetRotation(-18 * i);

            pb.Objects.Add(obj1);
            pb.Objects.Add(obj2);
            Angle += (float)Math.PI / 10;
        }

        pb.Export(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs");
    }
}
```

# Reference Sheet
Event:
|Class|Method|Parameters|Description|
|--|--|--|--|
|Event|Clone|None|Returns a deep copy of the calling object.|
|Event|ToString|None|Returns the calling object as a Json string.|
|GameObject|SetPosition|float X, float Y,|Sets the starting position event.|
|GameObject|SetPosition|float X, float Y, RandomMode Random, float RandomX, float RandomY, float RandomInterval|Sets the starting position event.|
|GameObject|SetScale|float X, float Y|Sets the starting position event.|
|GameObject|SetScale|float X, float Y, RandomMode Random, float RandomX, float RandomY, float RandomInterval|Sets the starting position event.|
|GameObject|SetRotation|float Angle|Sets the starting position event.|
|GameObject|SetRotation|float Angle RandomMode Random, float RandomAngle, float RandomInterval|Sets the starting position event.|
|GameObject|SetColor|float Color|Sets the starting position event.|
|GameObject|AddEvent|EventType Type, float Time, float X, float? Y, Easing Ease|Creates a new event of type T specified in the parameter Type.|
|GameObject|AddEvent|EventType Type, float Time, float X, float? Y, Easing Ease, RandomMode Random, float RandomX, float? RandomY, float RandomInterval|Creates a new event of type T specified in the parameter Type.|
|GameObject|RemoveEvent|EventType Type, int Index|Removes an event from the List of events with the type specified in Type at the index of Index|
|GameObject|Clone|None|Returns a deep copy of the calling object|
|GameObject|ToString|None|Returns the calling object as a Json string|
|PrefabBuilder|ImportPrefab|string Path|Takes all objects from the prefab specified in the Path variable and returns them. It will return a single GameObject if the prefab only contains one object, otherwise it will return a list of GameObjects|
|PrefabBuilder|ExportPrefab|string Path|Finalizes and exports the stored prefab in the calling object to a folder specified in the Path variable|
|PrefabBuilder|ToString|None|Returns the prefab stored in the calling object as a Json string|
