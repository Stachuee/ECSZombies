using Unity.Entities;
using UnityEngine;


public partial class GridInitializing : ComponentSystemGroup { }

[UpdateAfter(typeof(GridInitializing))]
public partial class GridBuilding : ComponentSystemGroup { }