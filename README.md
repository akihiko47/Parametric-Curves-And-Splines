# 〰️SPLINES〰️
In this repository, I have implemented a tool for creating and editing splines. The scripts give easy access to points on curve and the ability to get the position, tangent, normal and binormal at the desired point of the curve.

## Installation
There are 2 ways:
- import `Splines.unitypackage` via *Assets-Import* Package
- clone/download this repository and move the `Assets/Scripts/Curves` folder to your Unity project's Assets folder

## Curve types 🔢
The `Curve` class implements 3 matrices for different curve types. You can switch between types in real time using curves redactor.
1) **Bézier**
2) **B-spline**
3) **Cardinal**

![curves](https://github.com/akihiko47/Parametric-Curves-And-Splines/blob/main/Images/curves.gif)

## Redactor 🖌️
The redactor allows you to see the curve, normals, tangents and binormals. You can also change the location of control points and reset them.
Redactor is automatically connected to a GameObject that has a `Spline` component.

## Usage 🎮
You can see a simple example of how to use it in the [Curve Animator](https://github.com/akihiko47/Parametric-Curves-And-Splines/blob/main/Assets/Scripts/Curves/Curve%20Animator.cs) file. Curve information can be accesed using `P()` method of `Spline` class.

   ```sh
   spline.P(t, out Vector3 vertex, out Vector3 tangent, out Vector3 normal, out Vector3 binormal);
   ```

where `t` - position of point on curve [0 -> spline.GetMaxPointInd()]



## 🏗️ Work in progress... 🏗️