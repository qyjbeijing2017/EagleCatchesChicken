using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


#if UNITY_EDITOR
using UnityEditor;
#endif

#region ClassAttribute
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public abstract class XLSXClassAttribute : Attribute
{
    public virtual bool isReadOnly => false;
    public virtual bool isWriteOnly => false;
}


public class XLSXLocalAttribute : XLSXClassAttribute { }
public class XLSXReadOnlyCalssAttribute : XLSXClassAttribute
{
    public override bool isReadOnly => true;
}
public class XLSXWriteOnlyCalssAttribute : XLSXClassAttribute
{
    public override bool isWriteOnly => true;
}
public class XLSXIgnoreCalssAttribute : XLSXClassAttribute
{
    public override bool isReadOnly => true;
    public override bool isWriteOnly => true;
}

#endregion


#region FieldAttribute
[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public abstract class IXLSXFiledAttribute : Attribute
{
    public virtual bool isReadOnly => false;
    public virtual bool isWriteOnly => false;
    public virtual Func<object, string> writer { get; protected set; }
    public virtual Func<string, object> reader { get; protected set; }
    public virtual HashSet<Type> referenceTypes { get; protected set; }
}

public class XLSXReadOnlyAttribute : IXLSXFiledAttribute
{
    public override bool isReadOnly => true;
}
public class XLSXWriteOnlyAttribute : IXLSXFiledAttribute
{
    public override bool isWriteOnly => true;
}
public class XLSXIgnoreAttribute : IXLSXFiledAttribute
{
    public override bool isReadOnly => true;
    public override bool isWriteOnly => true;
}


#endregion

public class XLSXTools
{

#if UNITY_EDITOR
    public static string TypeToString(Type t, object instance)
    {

        if (t == typeof(Vector2))
        {
            var value = (Vector2)instance;
            return $"{value.x},{value.y}";
        }
        else if (t == typeof(Vector3))
        {
            var value = (Vector3)instance;
            return $"{value.x},{value.y},{value.z}";
        }
        else if (t == typeof(Vector4))
        {
            var value = (Vector4)instance;
            return $"{value.x},{value.y},{value.z},{value.w}";
        }
        else if (t == typeof(Quaternion))
        {
            var value = (Quaternion)instance;
            return $"{value.x},{value.y},{value.z},{value.w}";
        }
        else if (t == typeof(Color))
        {
            var value = (Color)instance;
            return $"{value.r},{value.g},{value.b},{value.a}";
        }
        else if (t == typeof(Color32))
        {
            var value = (Color32)instance;
            return $"{value.r},{value.g},{value.b},{value.a}";
        }
        else if (typeof(ScriptableObject).IsAssignableFrom(t))
        {
            try
            {
                var scriptable = (ScriptableObject)instance;
                if (scriptable == null) return "";
                var str = scriptable.name.Split('_')[1];
                return str;
            }
            catch (Exception e)
            {
                Debug.LogError($"ScriptableObject Translate Error: TypeName: {t.Name}, Instance: {instance}");
                throw e;
            }
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)instance;
            var str = "";
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                if (value != null)
                    str += TypeToString(list[i].GetType(), list[i]);
                if (i < list.Count - 1)
                {
                    str += "\n";
                }
            }
            return str;
        }
        else if (typeof(Enum).IsAssignableFrom(t))
        {
            return ((Enum)instance).ToString();
        }
        else if (t == typeof(string))
        {
            return (string)instance;
        }
        else if (t == typeof(int))
        {
            return ((int)instance).ToString();
        }
        else if (t == typeof(float))
        {
            return ((float)instance).ToString();
        }
        else if (t == typeof(bool))
        {
            return ((bool)instance).ToString();
        }
        else if (t == typeof(long))
        {
            return ((long)instance).ToString();
        }
        else if (instance == null)
        {
            return "";
        }
        else if (t == typeof(LayerMask))
        {
            var layerNames = "";
            var layerMask = (LayerMask)instance;
            var layerCode = layerMask.value;
            for (int i = 0; i < 32; i++)
            {
                if ((layerCode & (1 << i)) != 0)
                {
                    if (layerNames != "")
                    {
                        layerNames += "\n";
                    }
                    layerNames += LayerMask.LayerToName(i);
                }
                
            }
            return layerNames;

        }

        return JsonUtility.ToJson(instance);
    }

    public static object StringToType(Type t, string value, string configFolder = "")
    {

        if (t == typeof(Vector2))
        {
            var values = value.Split(',');
            return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        }
        else if (t == typeof(Vector3))
        {
            var values = value.Split(',');
            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        else if (t == typeof(Vector4))
        {
            var values = value.Split(',');
            return new Vector4(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Quaternion))
        {
            var values = value.Split(',');
            return new Quaternion(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Color))
        {
            var values = value.Split(',');
            return new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Color32))
        {
            var values = value.Split(',');
            return new Color32(byte.Parse(values[0]), byte.Parse(values[1]), byte.Parse(values[2]), byte.Parse(values[3]));
        }
        else if (typeof(ScriptableObject).IsAssignableFrom(t))
        {
            return AssetDatabase.LoadAssetAtPath($"{configFolder}/{t.Name.Replace("ScriptableObject", "")}_{value}.asset", t);
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)Activator.CreateInstance(t);
            var values = value.Split('\n');
            if (values.Length == 1 && values[0] == "")
            {
                return list;
            }
            for (int i = 0; i < values.Length; i++)
            {
                list.Add(StringToType(t.GetGenericArguments()[0], values[i], configFolder));
            }
            return list;
        }
        else if (typeof(Enum).IsAssignableFrom(t))
        {
            return Enum.Parse(t, value);
        }
        else if (t == typeof(string))
        {
            return value;
        }
        else if (t == typeof(int))
        {
            return int.Parse(value);
        }
        else if (t == typeof(float))
        {
            return float.Parse(value);
        }
        else if (t == typeof(bool))
        {
            return bool.Parse(value);
        }
        else if (t == typeof(long))
        {
            return long.Parse(value);
        }
        else if (value == "")
        {
            return null;
        }
        else if (t == typeof(LayerMask))
        {
            var layerMask = 0;
            var layerNames = value.Split('\n');
            foreach (var layerName in layerNames)
            {
                layerMask |= 1 << LayerMask.NameToLayer(layerName);
            }
            return new LayerMask { value = layerMask };
        }

        return JsonUtility.FromJson(value, t);
    }
#endif
    public static List<Type> AllTypes()
    {
        var types = typeof(ScriptableObject)
            .Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ScriptableObject)))
            .ToList();

        types.Sort((a, b) =>
        {
            foreach (var field in a.GetFields())
            {
                if (b.IsAssignableFrom(field.FieldType))
                {
                    return 1;
                }
                if (field.FieldType.IsGenericType)
                {
                    if (b.GetGenericArguments().Contains(field.FieldType)) return 1;
                }

                field.GetCustomAttributes<IXLSXFiledAttribute>();
                foreach (var attribute in field.GetCustomAttributes<IXLSXFiledAttribute>())
                {
                    if (attribute.referenceTypes.Contains(b)) return 1;
                }
            }

            foreach (var field in b.GetFields())
            {
                if (a.IsAssignableFrom(field.FieldType))
                {
                    return -1;
                }
                if (field.FieldType.IsGenericType)
                {
                    if (a.GetGenericArguments().Contains(field.FieldType)) return -1;
                }

                field.GetCustomAttributes<IXLSXFiledAttribute>();
                foreach (var attribute in field.GetCustomAttributes<IXLSXFiledAttribute>())
                {
                    if (attribute.referenceTypes.Contains(a)) return -1;
                }
            }

            return 0;
        });

        return types;
    }
}