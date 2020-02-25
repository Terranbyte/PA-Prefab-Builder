# Project Arrhythmia Prefab Builder
A C# library used to create prefabs in the game [Project Arrhythmia](https://store.steampowered.com/app/440310/Project_Arrhythmia/).

**Importaint!**
This library is unfinished, this means it has bugs, missing features and other things incomplete things don't have. This tutorial is also unfinished due to the library not being finished.
If you encounter any bugs/mistakes or you have a feature request, please DM me on Discord (Terranbyte#1691)
## Installing the library (VS 2019)

 1. Download the DLL from [here](https://github.com/Terranbyte/PA-Prefab-Builder/releases/download/v0.5/PrefabBuilder.dll).
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
When exporting a prefab, use the Export function on your PrefabBuilder object with the prefab folder as the parameter
```cs
pb.Export(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs");
```
To import prefabs call ImportPrefab on the PrefabBuilder class, then specify the path to the prefab you want to import.
```cs
pb.ImportPrefab(@"C:\Program Files (x86)\Steam\steamapps\common\Project Arrhythmia\beatmaps\prefabs\prefab.lsp");
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
