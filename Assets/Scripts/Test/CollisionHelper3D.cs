using System;
using System.Numerics;
using System.Collections.Generic;
using ZeroFrame.AllMath;
using ZeroFrame.AllPhysics;
using ZeroFrame.AllPhysics.Physics3D;

public static class CollisionHelper3D
    {

        #region [Penetration]

        public static Vector3 PenetrationCorrection(Box3D box1, Box3D box2)
        {
            var mtv = GetMTV(box1, box2);
            var newCenter = box1.Center + mtv;
            box1.UpdateCenter(newCenter);
            return mtv;
        }

        public static Vector3 GetMTV(Box3D box1, Box3D box2)
        {
            // 针对Box，求 3 + 3 = 6 个面的法向量上的投影，即12次投影计算
            var axis = box1.GetAxisX();
            var pjSub1 = box1.GetAxisX_SelfProjectionSub();
            var pjSub2 = box2.GetProjectionSub(axis);
            var l1 = pjSub2.Y - pjSub1.X;
            var l2 = pjSub1.Y - pjSub2.X;
            var lm = Math.Min(l1, l2);

            var len_min = lm;
            var dir = l1 < l2 ? axis.dir : -axis.dir;

            axis = box1.GetAxisY();
            pjSub1 = box1.GetAxisY_SelfProjectionSub();
            pjSub2 = box2.GetProjectionSub(axis);
            l1 = pjSub2.Y - pjSub1.X;
            l2 = pjSub1.Y - pjSub2.X;
            lm = Math.Min(l1, l2);
            if (lm < len_min)
            {
                len_min = lm;
                dir = l1 < l2 ? axis.dir : -axis.dir;
            }

            axis = box1.GetAxisZ();
            pjSub1 = box1.GetAxisZ_SelfProjectionSub();
            pjSub2 = box2.GetProjectionSub(axis);
            l1 = pjSub2.Y - pjSub1.X;
            l2 = pjSub1.Y - pjSub2.X;
            lm = Math.Min(l1, l2);
            if (lm < len_min)
            {
                len_min = lm;
                dir = l1 < l2 ? axis.dir : -axis.dir;
            }

            axis = box2.GetAxisX();
            pjSub2 = box2.GetAxisX_SelfProjectionSub();
            pjSub1 = box1.GetProjectionSub(axis);
            l1 = pjSub2.Y - pjSub1.X;
            l2 = pjSub1.Y - pjSub2.X;
            lm = Math.Min(l1, l2);
            if (lm < len_min)
            {
                len_min = lm;
                dir = l1 < l2 ? axis.dir : -axis.dir;
            }

            axis = box2.GetAxisY();
            pjSub2 = box2.GetAxisY_SelfProjectionSub();
            pjSub1 = box1.GetProjectionSub(axis);
            l1 = pjSub2.Y - pjSub1.X;
            l2 = pjSub1.Y - pjSub2.X;
            lm = Math.Min(l1, l2);
            if (lm < len_min)
            {
                len_min = lm;
                dir = l1 < l2 ? axis.dir : -axis.dir;
            }

            axis = box2.GetAxisZ();
            pjSub2 = box2.GetAxisZ_SelfProjectionSub();
            pjSub1 = box1.GetProjectionSub(axis);
            l1 = pjSub2.Y - pjSub1.X;
            l2 = pjSub1.Y - pjSub2.X;
            lm = Math.Min(l1, l2);
            if (lm < len_min)
            {
                len_min = lm;
                dir = l1 < l2 ? axis.dir : -axis.dir;
            }

            return len_min * dir;
        }

        #endregion

        public static bool HasCollision(Ray3D ray, Sphere3D sphere, out List<Vector3> hitPoints) => HasCollision(sphere, ray, out hitPoints);

        public static bool HasCollision(Sphere3D sphere, Ray3D ray, out List<Vector3> hitPoints)
        {
            hitPoints = new List<Vector3>();
            var sphereCenter = sphere.Center;
            var sphereRadius = sphere.Radius_scaled;
            var sphereRadius_sqr = sphereRadius * sphereRadius;
            var rayDir = ray.dir;
            var rayOrigin = ray.origin;
            var rayLen = ray.length;

            var t1 = Vector3.Dot(sphereCenter - rayOrigin, rayDir);

            var p1 = rayOrigin + t1 * rayDir;
            var d_sqr = Vector3.DistanceSquared(p1, sphereCenter);
            if (d_sqr > sphereRadius_sqr) return false;

            if (d_sqr == sphereRadius_sqr)
            {
                if (t1 <= rayLen) hitPoints.Add(p1);
                return false;
            }

            var diff_sqr = sphereRadius_sqr - d_sqr;
            var t_delta = (float)Math.Sqrt(diff_sqr);
            var t2 = (t1 - t_delta);
            var t3 = (t1 + t_delta);
            var p2 = rayOrigin + rayDir * t2;
            var p3 = rayOrigin + rayDir * t3;
            if (t2 <= rayLen) hitPoints.Add(p2);
            if (t3 <= rayLen) hitPoints.Add(p3);
            return true;
        }

        public static bool HasCollision(Ray3D ray, Box3D box, out List<Vector3> hitPoints) => HasCollision(box, ray, out hitPoints);

        // 算法解析：
        // ① 射线方程 R = Ro + t * Rd
        // ② 平面的特性: 在任意平面上,求得法向量(单位向量)D后，任意2个点P0,P1,都满足方程 P0 · D = P1 · D 
        // ③ 根据①②可求得 t = ( P0 · d - Ro · d ) / ( Rd · d ) = ( P0 - Ro ) · d / ( Rd · d )
        // 分别与AABB六个面进行射线交点检测,可求得t,再求得对应R值,即hitPoint
        public static bool HasCollision(Box3D box, Ray3D ray, out List<Vector3> hitPoints)
        {
            hitPoints = new List<Vector3>();

            var ro = ray.origin;
            var rd = ray.dir;
            var rl = ray.length;

            // AxisX's Two Planes
            var d = box.GetAxisX().dir.Normalize();
            var p0 = box.A;
            var t = Vector3.Dot(p0 - ro, d) / Vector3.Dot(rd, d);
            if (t > 0 && t <= rl)
            {
                var r = ro + t * rd;
                if (box.IsInside(r)) hitPoints.Add(r);
            }

            p0 = box.B;
            t = Vector3.Dot(p0 - ro, d) / Vector3.Dot(rd, d);
            if (t > 0 && t <= rl)
            {
                var r = ro + t * rd;
                if (box.IsInside(r)) hitPoints.Add(r);
            }

            // AxisY's Two Planes
            d = box.GetAxisY().dir.Normalize();
            p0 = box.A;
            t = Vector3.Dot(p0 - ro, d) / Vector3.Dot(rd, d);
            if (t > 0 && t <= rl)
            {
                var r = ro + t * rd;
                if (box.IsInside(r)) hitPoints.Add(r);
            }

            p0 = box.E;
            t = Vector3.Dot(p0 - ro, d) / Vector3.Dot(rd, d);
            if (t > 0 && t <= rl)
            {
                var r = ro + t * rd;
                if (box.IsInside(r)) hitPoints.Add(r);
            }

            // AxisZ's Two Planes
            d = box.GetAxisZ().dir.Normalize();
            p0 = box.A;
            t = Vector3.Dot(p0 - ro, d) / Vector3.Dot(rd, d);
            if (t > 0 && t <= rl)
            {
                var r = ro + t * rd;
                if (box.IsInside(r)) hitPoints.Add(r);
            }

            p0 = box.D;
            t = Vector3.Dot(p0 - ro, d) / Vector3.Dot(rd, d);
            if (t > 0 && t <= rl)
            {
                var r = ro + t * rd;
                if (box.IsInside(r)) hitPoints.Add(r);
            }

            return hitPoints.Count != 0;
        }

        public static bool HasCollision(Box3D box1, Box3D box2)
        {
            if (box1.BoxType == BoxType.OBB || box2.BoxType == BoxType.OBB) return HasCollision_OBB(box1, box2);
            else return HasCollision_AABB(box1, box2);
        }

        #region [Private]

        #region [AABB]

        static bool HasCollision_AABB(Box3D box1, Box3D box2)
        {
            var lufPos1 = box1.A;
            var rdbPos1 = box1.G;
            var lufPos2 = box2.A;
            var rdbPos2 = box2.G;

            // - Axis X
            var diff_x1 = lufPos1.X - rdbPos2.X;
            var diff_x2 = rdbPos1.X - lufPos2.X;
            bool hasCollisionX = (diff_x1 < 0 && diff_x2 > 0) || diff_x1 == 0 || diff_x2 == 0;

            // - Axis Y
            var diff_y1 = lufPos1.Y - rdbPos2.Y;
            var diff_y2 = rdbPos1.Y - lufPos2.Y;
            bool hasCollisionY = (diff_y1 > 0 && diff_y2 < 0) || diff_y1 == 0 || diff_y2 == 0;

            // - Axis Y
            var diff_z1 = lufPos1.Z - rdbPos2.Z;
            var diff_z2 = rdbPos1.Z - lufPos2.Z;
            bool hasCollisionZ = (diff_z1 > 0 && diff_z2 < 0) || diff_z1 == 0 || diff_z2 == 0;

            return hasCollisionX && hasCollisionY && hasCollisionZ;
        }

        static bool HasCollision_AABB(Vector3 maxPos1, Vector3 minPos1, Vector3 maxPos2, Vector3 minPos2)
        {

            // - Axis X
            var diff_x1 = maxPos1.X - minPos2.X;
            var diff_x2 = minPos1.X - maxPos2.X;
            bool hasCollisionX = (diff_x1 < 0 && diff_x2 > 0) || diff_x1 == 0 || diff_x2 == 0;

            // - Axis Y
            var diff_y1 = maxPos1.Y - minPos2.Y;
            var diff_y2 = minPos1.Y - maxPos2.Y;
            bool hasCollisionY = (diff_y1 > 0 && diff_y2 < 0) || diff_y1 == 0 || diff_y2 == 0;

            // - Axis Y
            var diff_z1 = maxPos1.Z - minPos2.Z;
            var diff_z2 = minPos1.Z - maxPos2.Z;
            bool hasCollisionZ = (diff_z1 > 0 && diff_z2 < 0) || diff_z1 == 0 || diff_z2 == 0;

            return hasCollisionX && hasCollisionY && hasCollisionZ;
        }

        #endregion

        #region [OBB]

        static bool HasCollision_OBB(Box3D box, Ray3D ray)
        {
            var rayOrigin = ray.origin;
            var rayDir = ray.dir;
            var rayLen = ray.length;
            var boxCenter = box.Center;

            // - Axis X
            var axis = box.GetAxisX();
            var pjSub1 = box.GetAxisX_SelfProjectionSub();
            var pjSub2 = ray.GetProjectionSub(axis);
            if (!HasIntersects(pjSub1, pjSub2)) return false;

            // - Axis Y  
            axis = box.GetAxisY();
            pjSub1 = box.GetAxisY_SelfProjectionSub();
            pjSub2 = ray.GetProjectionSub(axis);
            if (!HasIntersects(pjSub1, pjSub2)) return false;

            // - Axis Z
            axis = box.GetAxisZ();
            pjSub1 = box.GetAxisZ_SelfProjectionSub();
            pjSub2 = ray.GetProjectionSub(axis);
            if (!HasIntersects(pjSub1, pjSub2)) return false;

            return true;
        }

        static bool HasCollision_OBB(Box3D box1, Box3D box2)
        {
            // - 6 Axis
            if (!HasIntersects_WithBox1AxisX(box1, box2)) return false;
            if (!HasIntersects_WithBox1AxisY(box1, box2)) return false;
            if (!HasIntersects_WithBox1AxisZ(box1, box2)) return false;
            if (!HasIntersects_WithBox1AxisX(box2, box1)) return false;
            if (!HasIntersects_WithBox1AxisY(box2, box1)) return false;
            if (!HasIntersects_WithBox1AxisZ(box2, box1)) return false;

            // - 9 Axis
            Axis3D axis = new Axis3D();
            var b1AxisX = box1.GetAxisX();
            var b1AxisY = box1.GetAxisY();
            var b1AxisZ = box1.GetAxisZ();
            var b2AxisX = box2.GetAxisX();
            var b2AxisY = box2.GetAxisY();
            var b2AxisZ = box2.GetAxisZ();
            // - X Cross X
            var b1AxisX_cross_b2AxisX = Vector3.Cross(b1AxisX.dir, b2AxisX.dir);
            axis.dir = b1AxisX_cross_b2AxisX;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - X Cross Y
            var b1AxisX_cross_b2AxisY = Vector3.Cross(b1AxisX.dir, b2AxisY.dir);
            axis.dir = b1AxisX_cross_b2AxisY;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - X Cross Z
            var b1AxisX_cross_b2AxisZ = Vector3.Cross(b1AxisX.dir, b2AxisZ.dir);
            axis.dir = b1AxisX_cross_b2AxisZ;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - Y Cross X
            var b1AxisY_cross_b2AxisX = Vector3.Cross(b1AxisY.dir, b2AxisX.dir);
            axis.dir = b1AxisY_cross_b2AxisX;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - Y Cross Y
            var b1AxisY_cross_b2AxisY = Vector3.Cross(b1AxisY.dir, b2AxisY.dir);
            axis.dir = b1AxisY_cross_b2AxisY;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - Y Cross Z
            var b1AxisY_cross_b2AxisZ = Vector3.Cross(b1AxisY.dir, b2AxisZ.dir);
            axis.dir = b1AxisY_cross_b2AxisZ;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - Z Cross X
            var b1AxisZ_cross_b2AxisX = Vector3.Cross(b1AxisZ.dir, b2AxisX.dir);
            axis.dir = b1AxisZ_cross_b2AxisX;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - Z Cross Y
            var b1AxisZ_cross_b2AxisY = Vector3.Cross(b1AxisZ.dir, b2AxisY.dir);
            axis.dir = b1AxisZ_cross_b2AxisY;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;
            // - Z Cross Z
            var b1AxisZ_cross_b2AxisZ = Vector3.Cross(b1AxisZ.dir, b2AxisZ.dir);
            axis.dir = b1AxisZ_cross_b2AxisZ;
            if (!HasIntersects_WithAxis(box1, box2, axis)) return false;

            return true;
        }

        #endregion

        static bool HasIntersects_WithBox1AxisX(Box3D box1, Box3D box2)
        {
            var b1AxisX = box1.GetAxisX();
            var box1_projSub = box1.GetAxisX_SelfProjectionSub();
            var box2_projSub = box2.GetProjectionSub(b1AxisX);
            return HasIntersects(box1_projSub, box2_projSub);
        }

        static bool HasIntersects_WithBox1AxisY(Box3D box1, Box3D box2)
        {
            var b1_axisY = box1.GetAxisY();
            var box1_projSub = box1.GetAxisY_SelfProjectionSub();
            var box2_projSub = box2.GetProjectionSub(b1_axisY);
            return HasIntersects(box1_projSub, box2_projSub);
        }

        static bool HasIntersects_WithBox1AxisZ(Box3D box1, Box3D box2)
        {
            var b1_axisZ = box1.GetAxisZ();
            var box1_projSub = box1.GetAxisZ_SelfProjectionSub();
            var box2_projSub = box2.GetProjectionSub(b1_axisZ);
            return HasIntersects(box1_projSub, box2_projSub);
        }

        static bool HasIntersects_WithAxis(Box3D box1, Box3D box2, Axis3D axis)
        {
            var box1_projSub = box1.GetProjectionSub(axis);
            var box2_projSub = box2.GetProjectionSub(axis);
            return HasIntersects(box1_projSub, box2_projSub);
        }

        static bool HasIntersects(Vector2 sub1, Vector2 sub2)
        {
            bool cross = !(sub1.Y < sub2.X || sub2.Y < sub1.X);
            return cross;
        }

        #endregion

}
