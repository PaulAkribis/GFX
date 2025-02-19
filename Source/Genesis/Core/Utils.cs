﻿using BulletSharp.Math;
using Genesis.Graphics;
using Genesis.Math;
using Genesis.UI;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Genesis.Core
{
    /// <summary>
    /// Contains utility functions for various tasks within the Genesis.Core namespace.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Gets the current time in milliseconds.
        /// </summary>
        /// <returns>The current time in milliseconds.</returns>
        public static long GetCurrentTimeMillis()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Converts a Bitmap image to a Base64 string.
        /// </summary>
        /// <param name="bitmap">The Bitmap image to convert.</param>
        /// <returns>The Base64 string representation of the Bitmap image.</returns>
        public static String ConvertBitmapToBase64(Bitmap bitmap)
        {
            System.IO.MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            var base64 = Convert.ToBase64String(byteImage);
            return base64;
        }

        /// <summary>
        /// Converts a Base64 string to a Bitmap image.
        /// </summary>
        /// <param name="base64">The Base64 string to convert.</param>
        /// <returns>The Bitmap image decoded from the Base64 string.</returns>
        public static Bitmap ConvertBase64ToBitmap(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Bitmap image = (Bitmap)Bitmap.FromStream(ms, true);
            ms.Close();
            return image;
        }

        /// <summary>
        /// Gets the width of a string given the font size and spacing.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="spacing">The character spacing.</param>
        /// <returns>The width of the string.</returns>
        public static float GetStringWidth(String text, float fontSize, float spacing)
        {
            int chars = text.Length;
            float baseWidth = chars * fontSize;
            float spaceWidth = (float)(fontSize * spacing);
            float spacingWidth = spaceWidth * (chars - 1);
            return baseWidth - spacingWidth;
        }

        /// <summary>
        /// Gets the height of a string given the font size and spacing.
        /// </summary>
        /// <param name="text">The input string.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="spacing">The line spacing.</param>
        /// <returns>The height of the string.</returns>
        public static float GetStringHeight(String text, float fontSize, float spacing)
        {
            int lines = text.Split('\n').Length;
            float baseHeight = lines * fontSize;
            float spaceHeight = (float)(fontSize * spacing);
            float spacingHeight = spaceHeight * (lines - 1);
            return baseHeight;
        }

        /// <summary>
        /// Gets the bounding rectangle of a string given its location, text, font size, and spacing.
        /// </summary>
        /// <param name="location">The location of the string.</param>
        /// <param name="text">The input string.</param>
        /// <param name="fontSize">The font size.</param>
        /// <param name="spacing">The character spacing.</param>
        /// <returns>The bounding rectangle of the string.</returns>
        public static Rect GetStringBounds(Vec3 location, String text, float fontSize, float spacing)
        {
            Rect rect = new Rect();
            rect.X = location.X;
            rect.Y = location.Y;
            rect.Width = GetStringWidth(text, fontSize, spacing);
            rect.Height = fontSize;
            return rect;
        }

        /// <summary>
        /// Gets the vertically mirrored position of a reference vector relative to a camera.
        /// </summary>
        /// <param name="vref">The reference vector.</param>
        /// <param name="camera">The camera used for mirroring.</param>
        /// <returns>The vertically mirrored position vector.</returns>
        public static Vec3 GetVMirroredPosition(Vec3 vref, Camera camera)
        {
            return GetVMirroredPosition(vref.X, vref.Y, vref.Z, camera);
        }

        /// <summary>
        /// Gets the vertically mirrored position of a reference vector relative to a camera.
        /// </summary>
        /// <param name="x">The reference x vector.</param>
        /// <param name="y">The reference y vector</param>
        /// <param name="z">The reference z vector</param>
        /// <param name="camera">The camera used for mirroring.</param>
        /// <returns>The vertically mirrored position vector.</returns>
        public static Vec3 GetVMirroredPosition(float x, float y, float z, Camera camera)
        {
            float diff = camera.Location.Y - y;

            float newX = x;
            float newY = diff + camera.Location.Y;
            float newZ = z;

            Console.WriteLine("Diff " + diff + " Cam " + camera.Location.ToString() + " new y " + newY);

            return new Vec3(newX, newY, newZ);
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static float ToRadians(float degrees)
        {
            return (float)(System.Math.PI * degrees / 180.0);
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static float ToDegrees(float radians)
        {
            return radians * 180.0f / (float)System.Math.PI;
        }

        /// <summary>
        /// Calculates the front vector of a camera using its location and rotation.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <returns>The front vector of the camera.</returns>
        public static Vec3 CalculateCameraFront(Camera camera)
        {
            Vec3 ctarget = camera.Location + new Vec3(0f, 0f, -1f);
            Vec3 cfront = Vec3.Normalized(ctarget - camera.Location);
            return cfront;
        }

        /// <summary>
        /// Calculates the front vector of a camera using its rotation angles.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <returns>The front vector of the camera.</returns>
        public static Vec3 CalculateCameraFront2(Camera camera)
        {
            Vec3 direction = new Vec3(0f);
            direction.X = (float)(System.Math.Cos(Utils.ToRadians(camera.Rotation.Y)) * System.Math.Cos(Utils.ToRadians(camera.Rotation.X)));
            direction.Y = (float)System.Math.Sin(Utils.ToRadians(camera.Rotation.X));
            direction.Z = (float)(System.Math.Sin(Utils.ToRadians(camera.Rotation.Y)) * System.Math.Cos(Utils.ToRadians(camera.Rotation.X)));
            return Vec3.Normalized(direction);
        }

        /// <summary>
        /// Calculates the vector pointing forward from a given position and rotation.
        /// </summary>
        /// <param name="v">The starting position vector.</param>
        /// <param name="rotation">The rotation vector.</param>
        /// <param name="dist">The distance to move in the forward direction.</param>
        /// <returns>The resulting position vector.</returns>
        public static Vec3 ForwardVector(Vec3 v, Vec3 rotaion, float dist)
        {
            Vec3 direction = new Vec3(0f);
            direction.X = (float)(System.Math.Cos(Utils.ToRadians(rotaion.Y)) * System.Math.Cos(Utils.ToRadians(rotaion.X)));
            direction.Y = (float)System.Math.Sin(Utils.ToRadians(rotaion.X));
            direction.Z = (float)(System.Math.Sin(Utils.ToRadians(rotaion.Y)) * System.Math.Cos(Utils.ToRadians(rotaion.X)));
            Vec3 nDir = Vec3.Normalized(direction);
            return v + (nDir * dist);
        }

        /// <summary>
        /// Creates an empty normal map with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the normal map.</param>
        /// <param name="height">The height of the normal map.</param>
        /// <returns>An empty normal map.</returns>
        public static Bitmap CreateEmptyNormalMap(int width, int height)
        {
            Bitmap normalMap = new Bitmap(width, height);

            Color normalColor = Color.FromArgb(128, 128, 255); 

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    normalMap.SetPixel(x, y, normalColor);
                }
            }

            return normalMap;
        }

        /// <summary>
        /// Creates an empty texture with the specified width and height.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <returns>An empty texture.</returns>
        public static Bitmap CreateEmptyTexture(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            Color color = Color.FromArgb(255, 255, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Calculates a vector infront of the given point
        /// </summary>
        /// <param name="location">The source location</param>
        /// <param name="rotation">The rotation</param>
        /// <param name="dist">The distance</param>
        /// <returns>Front vector</returns>
        public static Vec3 CalculateFrontVec3(Vec3 location, Vec3 rotation, float dist)
        {
            Vec3 direction = new Vec3(0f);
            direction.X = (float)(System.Math.Cos(Utils.ToRadians(rotation.Y)) * System.Math.Cos(Utils.ToRadians(rotation.X)));
            direction.Y = (float)System.Math.Sin(Utils.ToRadians(rotation.X));
            direction.Z = (float)(System.Math.Sin(Utils.ToRadians(rotation.Y)) * System.Math.Cos(Utils.ToRadians(rotation.X)));

            return location + direction * dist;
        }

        /// <summary>
        /// Gets the transformation matrix for the parent model view.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <returns>The transformation matrix for the parent model view.</returns>
        public static mat4 GetParentModelView(GameElement element)
        {
            if (element.Parent != null) 
            {
                mat4 mt_mat = mat4.Translate(element.Parent.Location.X, element.Parent.Location.Y, element.Parent.Location.Z);
                mat4 mr_mat = mat4.RotateX(element.Parent.Rotation.X) * mat4.RotateY(element.Parent.Rotation.Y) * mat4.RotateZ(element.Parent.Rotation.Z);
                mat4 ms_mat = mat4.Scale(element.Parent.Size.X, element.Parent.Size.Y, element.Parent.Size.Z);
                return mt_mat * mr_mat * ms_mat;
            }
            return mat4.Identity;
        }

        /// <summary>
        /// Returns the world location for the game element.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <returns>The world location of the game element.</returns>
        public static Vec3 GetElementWorldLocation(GameElement element)
        {
            GameElement currentElement = element;
            Vec3 currentPosition = element.Location;
            while (currentElement.Parent != null)
            {
                quat parentRotation = Utils.EulerToQuaternion(currentElement.Parent.Rotation);
                vec3 rotatedPosition = parentRotation * currentPosition.ToGlmVec3();
                currentPosition = new Vec3(rotatedPosition) + currentElement.Parent.Location;
                currentElement = currentElement.Parent;
            }

            return currentPosition;
        }

        /// <summary>
        /// Returns the world rotation for the game element.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <returns>The world rotation of the game element.</returns>
        public static Vec3 GetElementWorldRotation(GameElement element)
        {
            GameElement currentElement = element;
            Vec3 currentRotation = element.Rotation;
            while(currentElement.Parent != null)
            {
                Vec3 parentRoation = currentElement.Parent.Rotation;
                currentRotation += parentRoation;
                currentElement = currentElement.Parent;
            }

            return currentRotation;
        }

        /// <summary>
        /// Returns the world scale for the game element.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <returns>The world scale of the game element.</returns>
        public static Vec3 GetElementWorldScale(GameElement element)
        {
            GameElement currentElement = element;
            Vec3 scale = currentElement.Size;

            while(currentElement.Parent != null)
            {
                Vec3 parentScale = currentElement.Parent.Size;
                scale *= parentScale;
                currentElement = currentElement.Parent;
            }

            return scale;
        }

        /// <summary>
        /// Converts the world transform to model space transform.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <param name="worldPosition">The world position vector.</param>
        /// <returns>The model space location of the game element.</returns>
        public static Vec3 GetModelSpaceLocation(GameElement element, Vec3 worldPosition)
        {
            GameElement currentElement = element;
            Vec3 modelPosition = worldPosition;
            while (currentElement.Parent != null)
            {
                Vec3 parentRotation = currentElement.Parent.Rotation;
                modelPosition -= currentElement.Parent.Location;
                vec3 modelPosv3 = Utils.EulerToQuaternion(parentRotation).Inverse * modelPosition.ToGlmVec3();
                modelPosition = new Vec3(modelPosv3);
                currentElement = currentElement.Parent;
            }

            return modelPosition;
        }

        /// <summary>
        /// Converts the world scale to the model space scale.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <param name="worldScale">The world scale vector.</param>
        /// <returns>The model space scale of the game element.</returns>
        public static Vec3 GetModelSpaceScale(GameElement element, Vec3 worldScale)
        {
            GameElement currentElement = element;
            Vec3 scale = worldScale;
            while(currentElement.Parent != null)
            {
                Vec3 parentScale = currentElement.Size;
                scale /= parentScale;
                currentElement = currentElement.Parent;
            }
            return scale;
        }

        /// <summary>
        /// Converts the world rotation to the model space rotation.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <param name="worldRotation">The world rotation vector.</param>
        /// <returns>The model space rotation of the game element.</returns>
        public static Vec3 GetModelSpaceRotation(GameElement element, Vec3 worldRotation)
        {
            GameElement currentElement = element;
            Vec3 rotation = worldRotation;
            while(currentElement.Parent != null)
            {
                rotation -= currentElement.Parent.Rotation;
                currentElement = currentElement.Parent;
            }
            return rotation;
        }

        /// <summary>
        /// Returns the model transform matrix relative to the world location.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <returns>The model transform matrix.</returns>
        public static mat4 GetModelTransformation(GameElement element)
        {
            Vec3 location = Utils.GetElementWorldLocation(element);
            return mat4.Translate(location.ToGlmVec3());
        }

        /// <summary>
        /// Returns the model rotation matrix relative to the world rotation.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <returns>The model rotation matrix.</returns>
        public static mat4 GetModelRotation(GameElement element)
        {
            Vec3 rotation = Utils.GetElementWorldRotation(element);
            quat quat = new quat(new vec3(Utils.ToRadians(rotation.X),Utils.ToRadians(rotation.Y), Utils.ToRadians(rotation.Z)));
            return new mat4(quat);
            //Changed this to radians
            //return mat4.RotateX(Utils.ToRadians(rotation.X)) * mat4.RotateY(Utils.ToRadians(rotation.Y)) * mat4.RotateZ(Utils.ToRadians(rotation.Z));
        }

        /// <summary>
        /// Returns the model scale matrix relative to the world scale.
        /// </summary>
        /// <param name="element">The game element.</param>
        /// <returns>The model scale matrix.</returns>
        public static mat4 GetModelScale(GameElement element)
        {
            Vec3 scale = Utils.GetElementWorldScale(element);
            return mat4.Scale(scale.X, scale.Y, scale.Z);
        }

        /// <summary>
        /// Converts Euler angles to a quaternion.
        /// </summary>
        /// <param name="euler">The Euler angles vector.</param>
        /// <returns>The quaternion representing the rotation.</returns>
        public static quat EulerToQuaternion(Vec3 euler)
        {
            quat quatX = quat.FromAxisAngle(euler.X, new vec3(1.0f, 0.0f, 0.0f));
            quat quatY = quat.FromAxisAngle(euler.Y, new vec3(0.0f, 1.0f, 0.0f));
            quat quatZ = quat.FromAxisAngle(euler.Z, new vec3(0.0f, 0.0f, 1.0f));

            return quatZ * quatY * quatX;
        }

        /// <summary>
        /// Calculates the direction vector from point A to point B.
        /// </summary>
        /// <param name="pointA">The starting point.</param>
        /// <param name="pointB">The target point.</param>
        /// <returns>The direction vector from point A to point B.</returns>
        public static Vec3 CalculateDirectionVector(Vec3 pointA, Vec3 pointB)
        {
            // Berechnung des Richtungsvektors
            return new Vec3(pointB.X - pointA.X, pointB.Y - pointA.Y, pointB.Z - pointA.Z);
        }

        /// <summary>
        /// Calculates the yaw angle (azimuth angle) from a given direction vector.
        /// </summary>
        /// <param name="directionVector">The direction vector.</param>
        /// <returns>The yaw angle in radians.</returns>
        static float CalculateYaw(Vec3 directionVector)
        {
            // Berechnung des Yaw-Winkels (Azimutwinkel)
            return (float)System.Math.Atan2(directionVector.Y, directionVector.X);
        }

        /// <summary>
        /// Calculates the pitch angle from a given direction vector.
        /// </summary>
        /// <param name="directionVector">The direction vector.</param>
        /// <returns>The pitch angle in radians.</returns>
        static float CalculatePitch(Vec3 directionVector)
        {
            // Berechnung des Pitch-Winkels
            return (float)System.Math.Atan2(-directionVector.Y, System.Math.Sqrt(directionVector.X * directionVector.X + directionVector.Z * directionVector.Z));
        }

        /// <summary>
        /// Lets the camera look at a position.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="targetPosition">The target position.</param>
        public static void LookAt(Camera camera, Vec3 targetPosition)
        {
            camera.Rotation.Y = Utils.CalculateYaw(camera.Location, targetPosition);
            camera.Rotation.X = Utils.CalculatePitch(camera.Location, targetPosition);
        }

        /// <summary>
        /// Calculates the yaw angle from point1 to point2.
        /// </summary>
        /// <param name="point1">The starting point.</param>
        /// <param name="point2">The target point.</param>
        /// <returns>The yaw angle in degrees.</returns>
        public static float CalculateYaw(Vec3 point1, Vec3 point2)
        {
            // Berechne die Differenzen zwischen den Koordinaten
            double deltaX = point2.X - point1.X;
            double deltaZ = point2.Z - point1.Z;

            // Verwende Atan2, um den Winkel zu berechnen (in Radian)
            float radians = (float)System.Math.Atan2(deltaZ, deltaX);

            // Konvertiere den Winkel von Radian nach Grad
            float angle = (float)(radians * (180 / System.Math.PI));

            return angle;
        }

        /// <summary>
        /// Calculates the pitch angle from point1 to point2.
        /// </summary>
        /// <param name="point1">The starting point.</param>
        /// <param name="point2">The target point.</param>
        /// <returns>The pitch angle in degrees.</returns>
        public static float CalculatePitch(Vec3 point1, Vec3 point2)
        {
            // Berechne die Differenzen zwischen den Koordinaten
            double deltaY = point2.Y - point1.Y;
            double horizontalDistance = System.Math.Sqrt((point2.X - point1.X) * (point2.X - point1.X) + (point2.Z - point1.Z) * (point2.Z - point1.Z));

            // Verwende Atan2, um den Pitch-Winkel zu berechnen (in Radian)
            float radians = (float)System.Math.Atan2(deltaY, horizontalDistance);

            // Konvertiere den Winkel von Radian nach Grad
            float pitch = (float)(radians * (180 / System.Math.PI));

            return pitch;
        }

        /// <summary>
        /// Converts a System.Drawing.Color into a float array.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The float array representing the color.</returns>
        public static float[] ConvertColor(Color color)
        {
            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;

            return new float[] { r, g, b };
        }

        /// <summary>
        /// Converts float values to a System.Drawing.Color.
        /// </summary>
        /// <param name="a">The alpha component.</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <returns>The System.Drawing.Color representation.</returns>
        public static Color ConvertDrawingColor(float a, float r, float g, float b)
        {
            return Color.FromArgb((int)a * 255, (int)r * 255, (int)g * 255, (int)b * 255);
        }

        /// <summary>
        /// Transforms screen coordinates to world coordinates in the context of a Game.
        /// </summary>
        /// <param name="game">The Game object.</param>
        /// <param name="x">X-coordinate on the screen.</param>
        /// <param name="y">Y-coordinate on the screen.</param>
        /// <returns>World coordinates as a Vec3.</returns>
        public static Vec3 TransformToWorldCords(Game game, float x, float y)
        {
            if(game.SelectedScene != null)
            {
                if(game.SelectedScene.Camera != null)
                {
                    float ndcX = (2.0f * x) / game.Viewport.Width - 1.0f;
                    float ndcY = (2.0f * y) / game.Viewport.Height - 1.0f;
                    var camera = game.SelectedScene.Camera;
                    mat4 p_mat;
                    mat4 v_mat;

                    if(camera.Type == CameraType.Perspective)
                    {
                        vec3 cameraPosition = camera.Location.ToGlmVec3();
                        Vec3 cameraFront = Utils.CalculateCameraFront2(camera);
                        p_mat = mat4.Perspective(Utils.ToRadians(45.0f), camera.Size.X / camera.Size.Y, camera.Near, camera.Far);
                        v_mat = mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));
                    }
                    else
                    {
                        float camX = camera.Location.X - (camera.Size.X / 2);
                        float camY = camera.Location.Y - (camera.Size.Y / 2);
                        float top = y + camera.Size.Y;
                        float right = x + camera.Size.X;
                        p_mat = mat4.Ortho(camX, right, camY, top, 0.0f, 100.0f);
                        v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));
                    }

                    mat4 viewProjectionMatrix = p_mat * v_mat;
                    mat4 inverseViewProjectionMatrix = viewProjectionMatrix.Inverse;
                    vec4 mousePositionNDC = new vec4(ndcX, ndcY, 0.0f, 1.0f);
                    vec4 worldPosition = inverseViewProjectionMatrix * mousePositionNDC;
                    
                    return new Vec3(worldPosition.x, worldPosition.y, worldPosition.z);
                }
            }
            return null;
        }

        /// <summary>
        /// Transforms screen coordinates to world coordinates in the context of a specific Camera and Viewport.
        /// </summary>
        /// <param name="camera">The Camera object.</param>
        /// <param name="viewport">The Viewport object.</param>
        /// <param name="x">X-coordinate on the screen.</param>
        /// <param name="y">Y-coordinate on the screen.</param>
        /// <returns>World coordinates as a Vec3.</returns>
        public static Vec3 TransformToWorldCords(Camera camera, Viewport viewport, float x, float y)
        {
            float ndcX = (2.0f * x) / viewport.Width - 1.0f;
            float ndcY = (2.0f * y) / viewport.Height - 1.0f;
            mat4 p_mat;
            mat4 v_mat;

            if (camera.Type == CameraType.Perspective)
            {
                vec3 cameraPosition = camera.Location.ToGlmVec3();
                Vec3 cameraFront = Utils.CalculateCameraFront2(camera);
                p_mat = mat4.Perspective(Utils.ToRadians(45.0f), camera.Size.X / camera.Size.Y, camera.Near, camera.Far);
                v_mat = mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));
            }
            else
            {
                float camX = camera.Location.X - (camera.Size.X / 2);
                float camY = camera.Location.Y - (camera.Size.Y / 2);
                float top = y + camera.Size.Y;
                float right = x + camera.Size.X;
                p_mat = mat4.Ortho(camX, right, camY, top, 0.0f, 100.0f);
                v_mat = mat4.LookAt(new vec3(0f, 0f, 1f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));
            }

            mat4 viewProjectionMatrix = p_mat * v_mat;
            mat4 inverseViewProjectionMatrix = viewProjectionMatrix.Inverse;
            vec4 mousePositionNDC = new vec4(ndcX, ndcY, 0.0f, 1.0f);
            vec4 worldPosition = inverseViewProjectionMatrix * mousePositionNDC;

            return new Vec3(worldPosition.x, worldPosition.y, worldPosition.z);
        }

        /// <summary>
        /// Calculates the direction of a ray in world space based on screen coordinates.
        /// </summary>
        /// <param name="camera">The Camera object.</param>
        /// <param name="viewport">The Viewport object.</param>
        /// <param name="x">X-coordinate on the screen.</param>
        /// <param name="y">Y-coordinate on the screen.</param>
        /// <returns>Direction of the ray as a Vec3.</returns>
        public static Vec3 RayDirection(Camera camera, Viewport viewport, float x, float y)
        {
            float ndcX = (2.0f * x) / viewport.Width - 1.0f;
            float ndcY = (2.0f * y) / viewport.Height - 1.0f;

            vec3 cameraPosition = camera.Location.ToGlmVec3();
            Vec3 cameraFront = Utils.CalculateCameraFront2(camera);
            var p_mat = mat4.Perspective(Utils.ToRadians(45.0f), camera.Size.X / camera.Size.Y, camera.Near, camera.Far);
            var v_mat = mat4.LookAt(cameraPosition, cameraPosition + cameraFront.ToGlmVec3(), new vec3(0.0f, 1.0f, 0.0f));

            mat4 viewProjectionMatrix = p_mat * v_mat;
            mat4 inverseViewProjectionMatrix = viewProjectionMatrix.Inverse;

            vec4 rayClip = new vec4(ndcX, ndcY, -1.0f, 1.0f);
            vec4 rayEye = inverseViewProjectionMatrix * rayClip;
            rayEye = new vec4(rayEye.x, rayEye.y, -1.0f, 0.0f);

            mat4 invViewMatrix = v_mat.Inverse;
            vec4 rayWorld = invViewMatrix * rayEye;

            vec3 rayDirection = glm.Normalized(new vec3(rayWorld.x, rayWorld.y, rayWorld.z));

            return new Vec3(rayDirection);
        }
    }
}
