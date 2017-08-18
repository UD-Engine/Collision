# UDEngine Collision Section  

This is the __Collision__ section of UDEngine, a possible Danmaku engine for __Touhou-like bullet hell games__ in *Unity3D*  

## Mechanism  
It implements a very basic Spatial-Hashing based collision detection mechanism. The `Collision/SpatialHash/SpatialHash.cs` shows a basic implementation of the grid based system. Currently, and considering the actual usage and usefulness, only circle collider is implemented under this system, and possibly __NO__ other collider would be implemented. (as many other collider shapes could be simulated with multiple circles, even for many rays (full screen rays could be implemented with builtin collider in Unity, as they are not costing too much performance))  

## Namespaces
The following namespaces is designed for better modularity:  
```bash
UDEngine # highest level namespace
UDEngine.Enum # all enum types are saved here
UDEngine.Interface # all interface types are saved here
UDEngine.Component # module for core components that are exposed to user
UDEnging.Component.Collision # most collision related classes are saved here
UDEngine.Internal # internal implementations, including the SpatialHash class itself. Also UDebug is saved here
```

## Implementation Structure
Implementation started with `SpatialHash`. Then the general collider interfaces and enums are defined. `UCircleCollider` is then implemented. `UTargetCollider` and `UBulletCollider` are implemented upon it as derived classes to enable callbacks. `UCollisionMonitor` is the actual supervisor and invoker of all the added callbacks, so it is implemented at last.   

## Performance
Using Spatial Hash and callback-invoke style improves the performance greatly. In effect, it could handle 2000 random bullet moving at 60 fps and 4000 at 30 fps (without any other rendering pressures added). In comparison, the builtin Circle2D collider runs only at 20 fps EVEN with only 200 bullets.

## Known issues
Using *Deep Profiling*, we found that `UMonolikeExecutor -> UCollisionMonitor.UpdateFunc() -> SpatialHash.Insert() -> SpatialHash.GetBucketIDs() -> SpatialHash.AddBucket() -> List.Contains() (especially this), List.Add(), Mathf.floor()` takes up most of the pressure. Also, the `UBulletCollider.InvokeDefaultCallbacks()` also takes a share. Optimization should start with them.  

For `List.Contains()`, a possible optimization is through using `HashSet.Contains()` instead. However, this requires complete reshaping of the internals of the `SpatialHash`.  

The `HashSet` version `USpatialHash()` has been implemented. However, no obvious performance gain is seen...
