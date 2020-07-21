﻿using UnityEngine;

namespace CustomCar
{
    public static class GameObjectEx
    {
        public static string FullName(this GameObject obj)
        {
            if (obj.transform.parent == null)
                return obj.name;
            return obj.transform.parent.gameObject.FullName() + "/" + obj.name;
        }

        public static string FullNameWithoutRoot(this GameObject obj)
        {
            if (obj.transform.parent == null)
                return "";
            return obj.transform.parent.gameObject.FullNameWithoutRoot() + obj.name;
        }
    }
}
