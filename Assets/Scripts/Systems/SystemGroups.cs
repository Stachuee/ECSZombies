using Unity.Entities;
using UnityEngine;


public partial class GridInitializing : ComponentSystemGroup { }

[UpdateAfter(typeof(GridInitializing)), UpdateAfter(typeof(GridBuilding))]
public partial class PhysicsInnitializing : ComponentSystemGroup { }


[UpdateAfter(typeof(GridInitializing))]
public partial class GridBuilding : ComponentSystemGroup { }

[UpdateAfter(typeof(GridBuilding))]
public partial class UnitMovment : ComponentSystemGroup { }

[UpdateAfter(typeof(UnitMovment))]

public partial class BuildingActions : ComponentSystemGroup { }

